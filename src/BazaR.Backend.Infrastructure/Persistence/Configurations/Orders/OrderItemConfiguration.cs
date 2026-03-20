using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        ConfigureKeys(builder);
        ConfigurePrimitiveProperties(builder);
        ConfigureMoney(builder);
        ConfigureIgnoredProperties(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .HasConversion(
                id => id.Value,
                value => new OrderId(value))
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .HasConversion(
                id => id.Value,
                value => new ProductId(value))
            .IsRequired();
    }

    private static void ConfigurePrimitiveProperties(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Property(x => x.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Sku)
            .HasColumnName("sku")
            .HasMaxLength(100);

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.CancelledQuantity)
            .HasColumnName("cancelled_quantity")
            .IsRequired();
    }

    private static void ConfigureMoney(EntityTypeBuilder<OrderItem> builder)
    {
        builder.OwnsOne(x => x.PriceSnapshot, money =>
        {
            money.WithOwner();

            money.Property(x => x.Amount)
                .HasColumnName("price_snapshot_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("price_snapshot_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.PriceSnapshot)
            .IsRequired();
    }

    private static void ConfigureIgnoredProperties(EntityTypeBuilder<OrderItem> builder)
    {
        builder.Ignore(x => x.ActiveQuantity);
        builder.Ignore(x => x.LineTotal);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("ix_order_items_order_id");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_order_items_product_id");

        builder.HasIndex(x => new { x.OrderId, x.ProductId })
            .HasDatabaseName("ix_order_items_order_id_product_id");
    }
}