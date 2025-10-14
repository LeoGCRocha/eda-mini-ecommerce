
using Catalog.Api;
using Orders.Api;

namespace EdaMicroEcommerce.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddModulesServices(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services
            .AddCatalog(appConfiguration)
            .AddOrders(appConfiguration);
        
        return services;
    }
}