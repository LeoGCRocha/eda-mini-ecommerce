using Catalog.Domain.Catalog;
using Catalog.Infra.Services;
using Catalog.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Catalog.Domain.Catalog.Products;
using Microsoft.Extensions.Configuration;
using EdaMicroEcommerce.Infra.Persistence;
using Catalog.Domain.Catalog.InventoryItems;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infra;

public static class CatalogServicesExtensions
{
    public static IServiceCollection AddCatalog(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDatabase(appConfiguration);
        services.AddProductInventoryServices();
        return services;
    }
    
    private static IServiceCollection AddProductInventoryServices(this IServiceCollection services)
    {
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
                .AddInterceptors(new DomainEventInterceptor()));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }
}