using EdaMicroEcommerce.Domain.BuildingBlocks.StronglyTyped;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Application.Saga;
using Orders.Application.Saga.Entity;
using Orders.Domain.Entities;

namespace Orders.Infra.Persistence;

public class SagaConfiguration : IEntityTypeConfiguration<SagaEntity>
{
    public void Configure(EntityTypeBuilder<SagaEntity> builder)
    {
        builder.ToTable("saga_entity");

        builder.HasKey(prop => prop.Id);

        builder.Property(prop => prop.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(prop => prop.OrderId)
            .HasConversion(v => v.Value,
                value => new OrderId(value))
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Property<DateTime>("created_at_utc")
            .ValueGeneratedOnAdd()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at_utc")
            .HasDefaultValueSql("NOW()");

        builder.Property<DateTime>("updated_at_utc")
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at_utc")
            .HasDefaultValueSql("NOW()");

        builder.HasKey(prop => prop.OrderId);

        builder.Property(prop => prop.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<Order>()
            .WithOne()
            .HasForeignKey<SagaEntity>(s => s.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}