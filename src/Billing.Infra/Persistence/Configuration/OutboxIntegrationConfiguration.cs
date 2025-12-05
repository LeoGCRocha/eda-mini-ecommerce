using Billing.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infras.Persistence.Configuration;

public class OutboxIntegrationConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent<EventType>>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent<EventType>> builder)
        => builder.CreateModuleOutboxConfiguration();
}