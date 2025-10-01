using EdaMicroEcommerce.Api.Features.Commands.Products.CreateProduct;
using EdaMicroEcommerce.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.IntegrationEvents.Products.ProductDeactivated;
using EdaMicroEcommerce.Domain.Catalog;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Domain.Catalog.Products;
using EdaMicroEcommerce.Infra.Configuration;
using EdaMicroEcommerce.Infra.Persistence;
using EdaMicroEcommerce.Infra.Repositories;
using EdaMicroEcommerce.Infra.Services;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.EntityFrameworkCore;

namespace EdaMicroEcommerce.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(ProductDeactivatedIntegrationHandler).Assembly);
        });

        return services;
    }

    public static IServiceCollection AddKafka(this IServiceCollection services,
        MessageBrokerConfiguration messageBrokerConfiguration)
    {
        // TODO Melhorar configurações do producer ver as melhores configs pro KAFKA em cada cenário
        if (!messageBrokerConfiguration.Producers.TryGetValue(MessageBrokerConst.ProductDeactivatedProducer,
                out var producerConfiguration))
            throw new ArgumentException("É esperado as configuração de producer para produto.");
            
        services.AddKafka(kafka =>
            kafka.UseConsoleLog()
                .AddCluster(cluster => cluster
                    .WithBrokers([messageBrokerConfiguration.BootstrapServers])
                    // product-deactivated
                    .CreateTopicIfNotExists(producerConfiguration.Topic, producerConfiguration.Partitions, producerConfiguration.ReplicaFactor)
                    .AddProducer(MessageBrokerConst.ProductDeactivatedProducer, producer =>
                    {
                        producer.DefaultTopic(producerConfiguration.Topic)
                            .AddMiddlewares(middlewares => middlewares.AddSerializer<JsonCoreSerializer>());
                    })
                )
        );
        
        return services;
    }
}