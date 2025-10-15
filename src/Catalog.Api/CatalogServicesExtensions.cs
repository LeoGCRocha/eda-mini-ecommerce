using Catalog.Api.CQS.Products.CreateProduct;
using Catalog.Application;
using Catalog.Application.IntegrationEvents;
using Catalog.Application.IntegrationEvents.Products.ProductDeactivated;
using Catalog.Contracts.Public;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.InventoryItems;
using Catalog.Domain.Entities.Products;
using Catalog.Infra;
using Catalog.Infra.Repositories;
using Catalog.Infra.Services;
using Confluent.Kafka;
using EdaMicroEcommerce.Infra.Configuration;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Api;

public static class CatalogServicesExtensions
{
    public static IServiceCollection AddCatalog(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDatabase(appConfiguration)
            .AddProductInventoryServices()
            .AddMediator(appConfiguration);
        return services;
    }
    
    private static IServiceCollection AddProductInventoryServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogApi, CatalogApi>();
        services.AddScoped<IProductInventoryService, ProductInventoryService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        return services;
    }
    
    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
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

    public static void RegisterProducers(IDictionary<string, ProducerConfiguration> producers, MessageBrokerConfiguration messageBrokerConfiguration)
    {
        if (!messageBrokerConfiguration.Producers.TryGetValue(MessageBrokerConst.ProductDeactivatedProducer,
                out var producerConfiguration))
            throw new ArgumentException("É esperado as configuração de producer para produto.");
        
        producers.Add(MessageBrokerConst.ProductDeactivatedProducer, new ProducerConfiguration
        {
            Topic = producerConfiguration.Topic,
            ReplicaFactor = producerConfiguration.ReplicaFactor,
            Partitions = producerConfiguration.Partitions
        });
    }
}
