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
using Platform.SharedContracts.IntegrationEvents.Payments;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddLogging(configure => configure.AddConsole());
        services.AddDatabase(configuration);

        services.AddSingleton<ISagaStateHandlerFactory, StateHandlerFactory>();
        services.AddScoped<ISagaStateHandler<OrderCreatedEvent>, OrderCreatedState>();
        services.AddScoped<ISagaStateHandler<ProductReservationStatusEvent>, ProductReservedState>();
        services.AddScoped<ISagaStateHandler<PaymentProcessedEvent>, PaymentProcessedState>();
        
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

        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.PaymentProcessedConsumer,
                out var paymentProcessedConsumer))
            throw new ArgumentException("It's expected a configuration to the consumer of PaymentProcessed.");

        services.AddKafkaFlowHostedService(kafka =>
        {
            kafka.UseMicrosoftLog();
            kafka.AddCluster(cluster =>
                cluster.WithBrokers([messageBroker.BootstrapServers])
                    .AddConsumer(consumer =>
                    {
                        consumer.Topic(consumerConfiguration.Topic);
                        consumer.WithGroupId(consumerConfiguration
                            .GroupId); 
                        consumer.WithWorkersCount(
                            1); 
                        consumer.WithBufferSize(
                            100); 
                        consumer.WithAutoOffsetReset(AutoOffsetReset
                            .Earliest); 

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
                            .GroupId); 
                        consumer.WithWorkersCount(
                            1); 
                        consumer.WithBufferSize(
                            100); 
                        consumer.WithAutoOffsetReset(AutoOffsetReset
                            .Earliest); 

                        consumer.AddMiddlewares(middlewares =>
                        {
                            middlewares.Add<MessageContextPropagationMiddleware>();
                            middlewares.Add<ProductReservedMiddleware>();
                        });
                    })
                    .AddConsumer(consumer =>
                    {
                        consumer.Topic(paymentProcessedConsumer.Topic);
                        consumer.WithGroupId(paymentProcessedConsumer.GroupId);
                        consumer.WithWorkersCount(1);
                        consumer.WithBufferSize(100);
                        consumer.WithAutoOffsetReset(AutoOffsetReset.Earliest);

                        consumer.AddMiddlewares(middlewares =>
                        {
                            middlewares.Add<MessageContextPropagationMiddleware>();
                            middlewares.Add<PaymentProcessedMiddleware>();
                        });
                    })
                );
        });
    }).Build();

await host.RunAsync();
