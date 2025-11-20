using Billing.Api;
using Billing.Application;
using Billing.Application.Observability;
using Billing.PaymentWorker.Extensions;
using Billing.PaymentWorker.IntegrationEvents;
using EdaMicroEcommerce.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow;
using KafkaFlow.Serializer;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;

        services.AddLogging(configure => configure.AddConsole());
        services.AddBilling(configuration);
        services.AddTelemetry(configuration);

        services.Configure<MessageBrokerConfiguration>(configuration.GetSection("MessageBroker"));

        var messageBrokerSection = configuration.GetSection("MessageBroker");

        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();

        if (messageBroker is null)
            throw new Exception("The message broker should be defined.");

        if (!messageBroker.Consumers.TryGetValue(MessageBrokerConst.PaymentPendingConsumer,
                out var paymentPendingConsumer))
            throw new ArgumentException(
                $"At least one configured expected to ${nameof(MessageBrokerConst.PaymentPendingConsumer)}");

        services.AddSingleton(Source.BillingSource);

        services.AddKafkaFlowHostedService(kafka =>
        {
            kafka.UseMicrosoftLog();
            kafka.AddCluster(cluster =>
                cluster.WithBrokers([messageBroker.BootstrapServers])
                    .AddConsumer(consumer =>
                    {
                        consumer.Topic(paymentPendingConsumer.Topic);
                        consumer.WithGroupId(paymentPendingConsumer.GroupId);
                        consumer.WithWorkersCount(1);
                        consumer.WithBufferSize(100);
                        consumer.WithAutoOffsetReset(AutoOffsetReset.Earliest);

                        consumer.AddMiddlewares(middlewares =>
                        {
                            middlewares.AddDeserializer<JsonCoreDeserializer>();
                            middlewares.Add<MessageContextPropagationMiddleware>();
                            middlewares.AddTypedHandlers(handlers =>
                            {
                                handlers.AddHandler<PaymentPendingMessageHandler>();
                                handlers.WhenNoHandlerFound(context =>
                                {
                                    Console.WriteLine("Message not handled > Partition {0} | Offset {1}",
                                        context.ConsumerContext.Partition,
                                        context.ConsumerContext.Offset);
                                });
                            });
                        });
                    })
            );
        });
    }).Build();

await host.RunAsync();