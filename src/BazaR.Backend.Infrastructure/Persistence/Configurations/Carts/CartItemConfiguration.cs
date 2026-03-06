using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("cart_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.CartId)
            .HasColumnName("cart_id")
            .HasConversion(
                id => id.Value,
                value => new CartId(value))
            .IsRequired();

        builder.HasIndex(x => x.CartId);

        builder.Property(x => x.OfferId)
            .HasColumnName("offer_id")
            .HasConversion(
                id => id.Value,
                value => new OfferId(value))
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(x => x.AddedAt)
            .HasColumnName("added_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.OwnsOne(x => x.PriceSnapshot, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("price_amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasIndex(x => new { x.CartId, x.OfferId }).IsUnique();
    }
}