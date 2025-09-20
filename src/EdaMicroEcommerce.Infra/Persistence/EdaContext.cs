using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Domain.Catalog.InventoryItems;
using EdaMicroEcommerce.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;

namespace EdaMicroEcommerce.Infra.Persistence;

public class EdaContext(DbContextOptions<EdaContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EdaContext).Assembly);
    }
}