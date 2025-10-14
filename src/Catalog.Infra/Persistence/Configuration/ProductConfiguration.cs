using Catalog.Domain.Entities.Products;
using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infra.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Id)
            .HasConversion(p => p.Value,
                value => new ProductId(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.Description);

        builder.Property(p => p.BasePrice)
            .HasColumnType("numeric (18, 2)");

        builder.Property(p => p.IsActive)
            .HasDefaultValueSql("TRUE");

        builder.Property<DateTime>("created_at_utc")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.Property<DateTime>("updated_at_utc")
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAddOrUpdate();

        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.IsActive);
    }
}