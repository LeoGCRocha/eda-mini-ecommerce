using EdaMicroEcommerce.Infra.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Api.CQS.CreateOrder;
using Orders.Application.IntegrationEvents;
using Orders.Application.IntegrationEvents.Orders;
using Orders.Domain.Services;
using Orders.Infra;
using Orders.Infra.Services;

namespace Orders.Api;

public static class OrderServicesExtensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services, IConfiguration appConfiguration)
    {
        var messageBrokerSection = appConfiguration.GetSection("MessageBroker");
        var messageBroker = messageBrokerSection.Get<MessageBrokerConfiguration>();
        if (messageBroker is null)
            throw new Exception("O Message Broker precisa estar definido corretamente.");

        services
            .AddDatabase(appConfiguration)
            .AddOrdersServices(appConfiguration)
            .AddMediator(appConfiguration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDbContext<OrderContext>(options =>
            options.UseNpgsql(appConfiguration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new DomainEventsInterceptor()));

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return services;
    }

    private static IServiceCollection AddOrdersServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }

    private static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(OrderCreatedIntegrationHandler).Assembly);
        });

        return services;
    }

    public static void RegisterProducers(IDictionary<string, ProducerConfiguration> producers,
        MessageBrokerConfiguration messageBrokerConfiguration)
    {
        if (!messageBrokerConfiguration.Producers.TryGetValue(MessageBrokerConst.OrderCreatedProducer,
                out var producerConfiguration))
            throw new ArgumentException("É esperado as configuração de producer para pedido.");

        if (!messageBrokerConfiguration.Producers.TryGetValue(MessageBrokerConst.ProductReservationProducer,
                out var productConfiguration))
            throw new ArgumentException("É esperado a configuração de producer para produtos.");

        producers.Add(MessageBrokerConst.OrderCreatedProducer, new ProducerConfiguration
        {
            Topic = producerConfiguration.Topic,
            ReplicaFactor = producerConfiguration.ReplicaFactor,
            Partitions = producerConfiguration.Partitions
        });
        
        // TODO: SUBIU COM APENAS UMA PARTIÇÃO
        producers.Add(MessageBrokerConst.ProductReservationProducer, new ProducerConfiguration()
        {
            Topic = productConfiguration.Topic,
            ReplicaFactor = productConfiguration.ReplicaFactor,
            Partitions = productConfiguration.Partitions
        });
    }
}