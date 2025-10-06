using EdaMicroEcommerce.Application.IntegrationEvents.Products.ProductDeactivated;
using EdaMicroEcommerce.Domain.Catalog;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Domain.Catalog.Products;
using EdaMicroEcommerce.Infra.Persistence;
using EdaMicroEcommerce.Infra.Repositories;
using EdaMicroEcommerce.Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdaMicroEcommerce.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddProductInventoryServices(this IServiceCollection services)
    {
        services.AddScoped<IProductInventoryService, ProductInventoryService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration appConfiguration)
    {
        services.AddDbContext<EdaContext>(options => 
            options.UseNpgsql(appConfiguration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new DomainEventInterceptor()));
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        return services;
    }
}