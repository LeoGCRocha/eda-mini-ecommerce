using EdaMicroEcommerce.Infra.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Api.CQS.CreateOrder;
using Orders.Infra;

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
                .UseSnakeCaseNamingConvention());
        // TODO: Add domain event interceptor
                // .AddInterceptors(new DomainEventsInterceptor()));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }
    
    private static IServiceCollection AddOrdersServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
    
    private static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);
        });
        
        return services;
    }
}