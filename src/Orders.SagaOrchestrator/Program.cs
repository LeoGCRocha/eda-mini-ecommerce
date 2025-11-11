using EdaMicroEcommerce.Application;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.IntegrationEvents;
using Orders.Application.Observability;
using Orders.Application.Repositories;
using Orders.Application.Saga;
using Orders.Application.Saga.States;
using Orders.Domain.Entities.Events;
using Orders.Infra;
using Orders.Infra.Repository;
using Orders.Saga.Extensions;
using Orders.Saga.IntegrationEvents;
using Orders.Saga.MessageMiddlewares;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddLogging(configure => configure.AddConsole());
        services.AddDatabase(configuration);

        services.AddSingleton<ISagaStateHandlerFactory, StateHandlerFactory>();
        services.AddScoped<ISagaStateHandler<OrderCreatedEvent>, OrderCreatedState>();
        services.AddScoped<ISagaStateHandler<ProductReservationStatusEvent>, ProductReservedState>();
        services.AddScoped<ISagaOrchestrator, SagaOrchestrator>();
        services.AddScoped<ISagaRepository, SagaRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton(Source.OrderSource);
        services.AddTelemetry(configuration);
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        services.Configure<MessageBrokerConfiguration>(configuration.GetSection("MessageBroker"));

        var messageBrokerSection = configuration.GetSection("MessageBroker");
        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
        if (messageBroker is null)
            throw new Exception("O Message Broker precisa estar definido corretamente.");

        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.OrderCreatedConsumer,
                out var consumerConfiguration))
            throw new ArgumentException("É esperado as configuração de consumer para produto.");
        
        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.ProductReservedConsumer,
                out var productReservedConsumer))
            throw new ArgumentException("É esperado as configuração de consumer para consume de reservas.");

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
                            middlewares.Add<MessageContextPropagationMiddleware>();
                            middlewares.AddTypedHandlers(handlers =>
                            {
                                handlers.WithHandlerLifetime(InstanceLifetime.Scoped);
                                handlers.AddHandler<OrderCreatedMessageHandler>();
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
                        consumer.Topic(productReservedConsumer.Topic);
                        consumer.WithGroupId(productReservedConsumer
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
                            middlewares.Add<MessageContextPropagationMiddleware>();
                            middlewares.Add<ProductReservedMiddleware>();
                        });
                    })
                );
        });
    }).Build();

await host.RunAsync();
