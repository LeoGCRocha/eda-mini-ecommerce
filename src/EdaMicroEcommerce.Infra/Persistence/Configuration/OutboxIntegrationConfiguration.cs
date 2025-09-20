using EdaMicroEcommerce.Infra.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EdaMicroEcommerce.Infra.Persistence.Configuration;

public class OutboxIntegrationConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder.ToTable("outbox_integration_events");

        builder.HasKey("id");
        
        builder.Property<int>("id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Type)
            .IsRequired();

        builder.Property(p => p.ProcessedAtUtc);

        builder.Property<DateTime>("created_at_utc")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.RetryCount)
            .HasDefaultValue(0);

        builder.Property(p => p.Payload)
            .IsRequired()
            .HasColumnType("jsonb");
    }
}