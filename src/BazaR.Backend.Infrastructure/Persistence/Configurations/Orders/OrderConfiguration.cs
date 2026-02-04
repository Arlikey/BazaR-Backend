using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Orders;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new OrderId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Ignore(x => x.Items);

        // Address VO
        builder.OwnsOne(x => x.DeliveryAddress, a =>
        {
            a.Property(p => p.Country).HasColumnName("country").HasMaxLength(100).IsRequired();
            a.Property(p => p.City).HasColumnName("city").HasMaxLength(100).IsRequired();
            a.Property(p => p.Street).HasColumnName("street").HasMaxLength(200).IsRequired();
            a.Property(p => p.Apartment).HasColumnName("apartment").HasMaxLength(50);
        });

        // Items: private List<OrderItem> _items
        builder.OwnsMany<OrderItem>("_items", items =>
        {
            items.ToTable("order_items");
            items.WithOwner().HasForeignKey("order_id");

            items.Property(x => x.Id).HasColumnName("id");
            items.HasKey(x => x.Id);

            items.Property(x => x.ProductId)
                .HasColumnName("product_id")
                .HasConversion(id => id.Value, value => new ProductId(value))
                .IsRequired();

            items.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            items.OwnsOne(x => x.PriceSnapshot, price =>
            {
                price.Property(p => p.Amount).HasColumnName("price_amount").IsRequired();
                price.Property(p => p.Currency).HasColumnName("price_currency").HasMaxLength(3).IsRequired();
            });

            items.HasIndex("order_id", nameof(OrderItem.ProductId));

        });

        builder.Navigation("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
