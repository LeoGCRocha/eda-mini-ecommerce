using Catalog.Application.IntegrationEvents;
using EdaMicroEcommerce.Application.Outbox;
using EdaMicroEcommerce.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infra.Persistence.Configuration;

public class OutboxIntegrationConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent<EventType>>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent<EventType>> builder)
        => builder.CreateModuleOutboxConfiguration();
}