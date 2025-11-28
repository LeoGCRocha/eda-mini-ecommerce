using Billing.Api.CQS;
using Billing.Application;
using Billing.Application.IntegrationEvents.Payment;
using Billing.Infras;
using Microsoft.EntityFrameworkCore;
using Billing.Application.Repositories;
using Billing.Application.Services;
using Billing.Application.Strategy;
using Billing.Application.Strategy.Interfaces;
using Billing.Infras.Repository;
using Billing.Infras.Services;
using EdaMicroEcommerce.Infra.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Api;

public static class BillingServiceExtensions
{
    public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services
            .AddDatabase(appConfiguration)
            .AddMediator(appConfiguration)
            .AddServices(appConfiguration);

        return services;
    }
    
    private static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ProcessPaymentCommandHandler).Assembly);
            cfg.RegisterServicesFromAssemblies(typeof(PaymentProcessedIntegrationEvent).Assembly);
        });

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDbContext<BillingContext>(options =>
            options.UseNpgsql(appConfiguration.GetConnectionString("EdaMicroDb"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new DomainEventsInterceptor()));

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddScoped<PaymentWithFeeStrategy>();
        services.AddScoped<PaymentWithoutFeeStrategy>();
        services.AddScoped<PaymentWithCouponsStrategy>();
        services.AddScoped<PaymentWithoutCouponStrategy>();
        services.AddScoped<IPaymentStrategyFactory, PaymentStrategyFactory>();
        
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ICouponsRepository, CouponRepository>();
        services.AddScoped<IPaymentCouponService, PaymentCouponService>();
        
        return services;
    }
    
    public static void RegisterProducers(IDictionary<string, ProducerConfiguration> producers,
        MessageBrokerConfiguration messageBrokerConfiguration)
    {
        if (!messageBrokerConfiguration.Producers.TryGetValue(MessageBrokerConst.PaymentProcessedProducer,
                out var producerConfiguration))
            throw new ArgumentException("Is expected a configuration to the producer.");
        
        producers.Add(MessageBrokerConst.PaymentProcessedProducer, new ProducerConfiguration
        {
            Topic = producerConfiguration.Topic,
            ReplicaFactor = producerConfiguration.ReplicaFactor,
            Partitions = producerConfiguration.Partitions
        });
        
    }
}