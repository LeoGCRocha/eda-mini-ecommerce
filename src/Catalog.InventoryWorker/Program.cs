using KafkaFlow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Catalog.Application.IntegrationEvents;
using Catalog.Application.Observability;
using EcaMicroEcommerce.ProductWorker;
using EcaMicroEcommerce.ProductWorker.Extensions;
using Microsoft.Extensions.DependencyInjection;
using EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductDeactivated;
using EdaMicroEcommerce.Application;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow.Serializer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddLogging(configure => configure.AddConsole());
        services.AddDatabase(configuration);
        services.AddProductInventoryServices();
        services.AddTelemetry(configuration);
        
        services.Configure<MessageBrokerConfiguration>(configuration.GetSection("MessageBroker"));

        var messageBrokerSection = configuration.GetSection("MessageBroker");
        
        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
        
        if (messageBroker is null)
            throw new Exception("O Message Broker precisa estar definido corretamente.");

        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.ProductDeactivatedConsumer,
                out var productDeactivatedConsumer))
            throw new ArgumentException("É esperado as configuração de consumer para produto.");
        
        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.InventoryReservationConsumer,
                out var productReservationConsumer))
            throw new ArgumentException("É esperado as configuração de consumer para produto.");

        services.AddSingleton(Source.CatalogSource);
        services.AddKafkaFlowHostedService(kafka =>
        {
            // <WARNING> Em termos práticos fiz tudo com auto commit habilitado, mas pode não ser a melhor pratica num
            // sistema real
            kafka.UseMicrosoftLog();
            kafka.AddCluster(cluster =>
                cluster.WithBrokers([messageBroker.BootstrapServers])
                    .AddConsumer(consumer =>
                    {
                        // TODO: Adicionar isso aqui no SAGA....
                        consumer.Topic(productDeactivatedConsumer.Topic);
                        consumer.WithGroupId(productDeactivatedConsumer
                            .GroupId); // Consumer group usado para definir o recebimento de mensagem
                        // TRAZER ISSO PARA AS CONFIGURAÇÕES
                        consumer.WithWorkersCount(
                            1); // Quantidade de threads paralelas que processam, obs, cada thread so obtem dados da mesma partição
                        consumer.WithBufferSize(
                            100); // Define o tamanho da fila interna (buffer) de mensagens que o KafkaFlow mantém
                        consumer.WithAutoOffsetReset(AutoOffsetReset
                            .Earliest); // Recebe mensagens do início do log de offset disponivel
                        
                        // TODO: Adicionar um consumo em BATCH EM ALGUM LUGAR QUE FAÇA SENTIDO PRA ESTRESSAR A LIB
                        consumer.AddMiddlewares(middlewares =>
                        {
                            middlewares.AddDeserializer<JsonCoreDeserializer>();
                            middlewares.Add<MessageContextPropagationMiddleware>();
                            middlewares.AddTypedHandlers(handlers =>
                            {
                                handlers.AddHandler<ProductDeactivatedMessageHandler>();
                                handlers.WhenNoHandlerFound(context =>
                                {
                                    Console.WriteLine("Mensagem não gerenciada > Partição {0} | Offset {1}",
                                        context.ConsumerContext.Partition,
                                        context.ConsumerContext.Offset);
                                });
                            });
                        });
                    })
                    .AddConsumer(consumer =>
                    {
                        consumer.Topic(productReservationConsumer.Topic);
                        consumer.WithGroupId(productReservationConsumer.GroupId);
                        consumer.WithWorkersCount(1);
                        consumer.WithBufferSize(100);
                        consumer.WithAutoOffsetReset(AutoOffsetReset.Earliest);

                        consumer.AddMiddlewares(middlewares =>
                        {
                            middlewares.Add<MessageContextPropagationMiddleware>();
                           // <WARNING> Tive problemas com o JSON CORE DESERIALIZER 
                           middlewares.Add<ProductReservationMiddleware>();
                        });
                    })
                );
        });
    }).Build();

await host.RunAsync();
