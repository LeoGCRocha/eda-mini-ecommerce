using Catalog.Domain.Entities.InventoryItems;
using Catalog.Domain.Entities.Products;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
    }
}