using Catalog.Api.CQS.Products.CreateProduct;
using Catalog.Application.IntegrationEvents;
using Catalog.Application.IntegrationEvents.Products.ProductDeactivated;
using Catalog.Domain.Catalog;
using Catalog.Infra.Services;
using Catalog.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Catalog.Domain.Catalog.Products;
using Microsoft.Extensions.Configuration;
using Catalog.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infra;

public static class CatalogServicesExtensions
{
    public static IServiceCollection AddCatalog(this IServiceCollection services, IConfiguration appConfiguration)
    {
        // <TIP> kafka-topics --delete --topic product-deactivated --bootstrap-server localhost:9092
        var messageBrokerSection = appConfiguration.GetSection("MessageBroker");
        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
        if (messageBroker is null)
            throw new Exception("O Message Broker precisa estar definido corretamente.");
        
        services.AddDatabase(appConfiguration)
            .AddProductInventoryServices()
            .AddMediator(appConfiguration)
            .AddMessageBroker(messageBroker);
        return services;
    }
    
    public static IServiceCollection AddProductInventoryServices(this IServiceCollection services)
    {
        services.AddScoped<IProductInventoryService, ProductInventoryService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        return services;
    }
    
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDbContext<CatalogContext>(options =>
            options.UseNpgsql(appConfiguration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new DomainEventsInterceptor()));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }

    private static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(ProductDeactivatedIntegrationHandler).Assembly);
        });

        return services;
    }

    private static IServiceCollection AddMessageBroker(this IServiceCollection services,
        MessageBrokerConfiguration messageBrokerConfiguration)
    {
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