using Billing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Billing.Infras.Persistence.Configuration;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired();

        builder.HasKey(p => new
        {
            p.Name,
            p.IsActive
        });

        builder.Property(p => p.DiscountPercentage)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.Property(p => p.ValidUntilUtl);
    }
}