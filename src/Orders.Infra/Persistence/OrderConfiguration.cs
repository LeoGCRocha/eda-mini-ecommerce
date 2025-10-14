using Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Orders.Infra.Persistence;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(property => property.Id);

        builder.Property(prop => prop.Id)
            .HasConversion(
                v => v.Value,
                value => new OrderId(value))
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(prop => prop.CustomerId)
            .HasConversion(
                v => v.Value,
                dbValue => new CustomerId(dbValue))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(prop => prop.TotalAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(prop => prop.Currency)
            .HasDefaultValue("BRL");

        builder.Property(prop => prop.DiscountAmount)
            .HasColumnType("numeric(18,2)")
            .HasDefaultValue(0);

        builder.Property(prop => prop.NetAmount)
            .HasColumnType("numeric(18,2)")
            .IsRequired();
        
        builder.Property(prop => prop.Status)
            .IsRequired();

        builder.Property(prop => prop.PaymentId)
            .HasConversion<Guid?>(
                v => v == null ? null : v.Value,
                dbValue => dbValue == null ? null : new PaymentId((Guid) dbValue))
            .ValueGeneratedNever()
            .IsRequired(false);
        
        builder.Property<DateTime>("created_at_utc")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.Property<DateTime>("updated_at_utc")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(prop => prop.PaymentDate)
            .IsRequired(false);

        builder.HasIndex(prop => prop.CustomerId);

        builder.HasIndex(prop => prop.PaymentDate);

        builder.OwnsMany(prop => prop.OrderItems, innerTable =>
        {
            innerTable.ToTable("order_items");

            innerTable.HasKey(prop => prop.Id);

            innerTable.Property(prop => prop.Id)
                .HasConversion(
                    v => v.Value,
                    value => new OrderItemId(value))
                .IsRequired()
                .ValueGeneratedNever();

            innerTable.Property(prop => prop.Quantity)
                .IsRequired();

            innerTable.Property(prop => prop.UnitPrice)
                .IsRequired()
                .HasColumnType("numeric(18,2)");

            innerTable.Property(prop => prop.ProductId)
                .HasConversion(
                    v => v.Value,
                    value => new ProductId(value))
                .IsRequired()
                .ValueGeneratedNever();

            innerTable.HasIndex(prop => prop.ProductId);
        });
    }
}