using KafkaFlow;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Catalog.Application.IntegrationEvents;
using EcaMicroEcommerce.ProductWorker;
using Microsoft.Extensions.DependencyInjection;
using EcaMicroEcommerce.ProductWorker.IntegrationsEvent.ProductDeactivated;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow.Serializer;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddLogging(configure => configure.AddConsole());
        services.AddDatabase(configuration);
        services.AddProductInventoryServices();
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services.Configure<MessageBrokerConfiguration>(configuration.GetSection("MessageBroker"));

        var messageBrokerSection = configuration.GetSection("MessageBroker");
        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
        if (messageBroker is null)
            throw new Exception("O Message Broker precisa estar definido corretamente.");

        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.ProductDeactivatedCosnumer,
                out var consumerConfiguration))
            throw new ArgumentException("É esperado as configuração de consumer para produto.");

        services.AddKafkaFlowHostedService(kafka =>
        {
            kafka.UseMicrosoftLog();
            kafka.AddCluster(cluster =>
                cluster.WithBrokers([messageBroker.BootstrapServers])
                    .AddConsumer(consumer =>
                    {
                        consumer.Topic(consumerConfiguration.Topic);
                        consumer.WithGroupId(consumerConfiguration
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
                            middlewares.AddTypedHandlers(handlers =>
                            {
                                handlers.WithHandlerLifetime(InstanceLifetime.Scoped);
                                handlers.AddHandler<ProductDeactivatedMessageHandler>();
                                handlers.WhenNoHandlerFound(context =>
                                {
                                    Console.WriteLine("Mensagem não gerenciada > Partição {0} | Offset {1}",
                                        context.ConsumerContext.Partition,
                                        context.ConsumerContext.Offset);
                                });
                            });
                        });
                    }));
        });
    }).Build();

await host.RunAsync();
