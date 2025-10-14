using Catalog.Application.IntegrationEvents;
using Catalog.Domain.Catalog.InventoryItems;
using Catalog.Domain.Catalog.Products;
using EdaMicroEcommerce.Application.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infra;

public class CatalogContext(DbContextOptions<CatalogContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }

    public DbSet<OutboxIntegrationEvent<EventType>> OutboxIntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }
}