using Catalog.Application.IntegrationEvents;
using Catalog.Domain.Entities.InventoryItems;
using Catalog.Domain.Entities.Products;
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