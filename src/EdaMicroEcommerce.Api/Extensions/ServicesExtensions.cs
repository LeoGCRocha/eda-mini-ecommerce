using Catalog.Infra;

namespace EdaMicroEcommerce.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddModulesServices(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddCatalog(appConfiguration);
        
        return services;
    }
}