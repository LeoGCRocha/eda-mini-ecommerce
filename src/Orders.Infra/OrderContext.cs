using Microsoft.EntityFrameworkCore;
using Orders.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.Outbox;
using Orders.Domain.Entities;

namespace Orders.Infra;

public class OrderContext(DbContextOptions<OrderContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    
    // TODO: Isso não deveria ser acessivel via EF talvez, e sim via DAPPER no Outbox?
    public DbSet<OutboxIntegrationEvent<EventType>> OutboxIntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderContext).Assembly);
    }
}