using Catalog.Infra;
using Catalog.Domain.Entities;
using Catalog.Domain.Entities.InventoryItems;
using Catalog.Domain.Entities.Products;
using Catalog.Infra.Repositories;
using Catalog.Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcaMicroEcommerce.ProductWorker;

public static class CatalogInventoryWorkerServiceExtensions
{
    internal static IServiceCollection AddProductInventoryServices(this IServiceCollection services)
    {
        services.AddScoped<IProductInventoryService, ProductInventoryService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        return services;
    }
    
    internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDbContext<CatalogContext>(options =>
            options.UseNpgsql(appConfiguration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new DomainEventsInterceptor()));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }
}