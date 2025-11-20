using Billing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Billing.Application.Repositories;
using Billing.Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Api;

public static class BillingServiceExtensions
{
    public static IServiceCollection AddBilling(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services
            .AddDatabase(appConfiguration)
            .AddServices(appConfiguration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDbContext<BillingContext>(options =>
            options.UseNpgsql(appConfiguration.GetConnectionString("EdaMicroDb"))
                .UseSnakeCaseNamingConvention());
        // TODO: Implement save changes intercept
        // .AddInterceptors(new DomainEventsInterceptor()));

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        return services;
    }
}