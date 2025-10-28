using Microsoft.EntityFrameworkCore;
using Catalog.Domain.Entities.Products;
using Catalog.Domain.Entities.InventoryItems;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;

namespace Catalog.Infra.Persistence.Configuration;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("inventory_items");

        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasConversion(p => p.Value,
                value => new InventoryItemId(value))
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(p => p.ProductId)
            .HasConversion(p => p.Value,
                value => new ProductId(value))
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(p => p.AvailableQuantity)
            .IsRequired();

        builder.Property(p => p.ReservedQuantity)
            .HasDefaultValue(0);

        builder.Property(p => p.ReorderLevel)
            .HasDefaultValue(0);

        builder.HasIndex(p => p.ProductId).IsUnique();

        builder.HasOne<Product>()
            .WithOne()
            .HasForeignKey<InventoryItem>(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(prop => prop.Reservations, innerTable =>
        {
            // <WARNING> Importante é que o owns many apesar de criar uma relação de posse entre o pai e filho
            // no ef possui uma limitação onde ele não define a constraint manualmente para o comportamento on delete.
            innerTable.ToTable("reservations");

            innerTable.Property<int>("id")
                .ValueGeneratedOnAdd();
            
            innerTable.HasKey("id");

            innerTable.Property(innerProp => innerProp.Quantity)
                .IsRequired()
                .HasDefaultValue(0);
            
            innerTable.Property(innerProp => innerProp.OccuredAtUtc)
                .HasDefaultValueSql("NOW()");
            
            innerTable.Property(innerProp => innerProp.OrderId)
                .HasConversion(
                    v => v.Value,
                    value => new OrderId(value))
                .IsRequired()
                .ValueGeneratedNever();

            innerTable.Property(innerProp => innerProp.Status)
                .HasConversion<string>()
                .HasColumnName("status")
                .IsRequired();

            innerTable.HasIndex(innerProp => innerProp.OrderId);
        });
    }
}