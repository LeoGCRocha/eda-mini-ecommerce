using Billing.Application.IntegrationEvents;
using Billing.Domain.Entities;
using EdaMicroEcommerce.Application.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Billing.Infrastructure;

public class BillingContext(DbContextOptions<BillingContext> options) : DbContext(options)
{
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<OutboxIntegrationEvent<EventType>> OutboxIntegrationEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("billing");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingContext).Assembly);
    }
}