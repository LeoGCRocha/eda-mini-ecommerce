using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Billing.Infras.Persistence.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.Id)
            .HasConversion(p => p.Value,
                p => new PaymentId(p))
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CustomerId)
            .HasConversion(p => p.Value,
                value => new CustomerId())
            .ValueGeneratedNever();
        
        builder.Property(p => p.Status)
            .HasConversion<string>();

        builder.Property(p => p.NetAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(p => p.GrossAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(p => p.FeeAmount)
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0.0);

        builder.Property(p => p.DiscountReason);

        builder.Property(p => p.OrderId)
            .HasConversion(p => p.Value,
                value => new OrderId(value))
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Property<DateTime>("created_at_utc")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.Property<DateTime>("updated_at_utc")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(p => p.OrderId);

        builder.Property<uint>("xmin")
            .IsRowVersion();
    }
}