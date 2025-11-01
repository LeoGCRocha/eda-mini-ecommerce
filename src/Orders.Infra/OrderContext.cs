using Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Saga.Entity;
using Orders.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.Outbox;

namespace Orders.Infra;

public class OrderContext(DbContextOptions<OrderContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<SagaEntity> Saga { get; set; }
    public DbSet<OutboxIntegrationEvent<EventType>> OutboxIntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderContext).Assembly);
    }
}