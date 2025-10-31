using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Application.IntegrationEvents;

namespace Orders.Infra.Persistence;

public class OutboxIntegrationConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent<EventType>>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent<EventType>> builder)
        => builder.CreateModuleOutboxConfiguration();
}