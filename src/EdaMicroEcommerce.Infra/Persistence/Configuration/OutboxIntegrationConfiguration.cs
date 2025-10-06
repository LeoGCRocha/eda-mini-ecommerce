using EdaMicroEcommerce.Application.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EdaMicroEcommerce.Infra.Persistence.Configuration;

public class OutboxIntegrationConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder.ToTable("outbox_integration_events");

        builder.Property<int>("id")
            .ValueGeneratedOnAdd();
        
        builder.HasKey("id");

        builder.Property(p => p.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.IsDeadLetter)
            .HasDefaultValue(false);

        builder.Property(p => p.ProcessedAtUtc);

        builder.Property(p => p.CreatedAtUtc)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.RetryCount)
            .HasDefaultValue(0);

        builder.Property(p => p.Payload)
            .IsRequired()
            .HasColumnType("jsonb");
    }
}