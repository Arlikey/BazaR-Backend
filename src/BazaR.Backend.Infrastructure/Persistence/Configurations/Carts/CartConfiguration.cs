using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Cart;

public sealed class CartConfiguration : IEntityTypeConfiguration<BazaR.Backend.Domain.Carts.Cart>
{
    public void Configure(EntityTypeBuilder<BazaR.Backend.Domain.Carts.Cart> builder)
    {
        builder.ToTable("carts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new CartId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(id => id.Value, value => new UserId(value))
            .IsRequired();

        builder.HasIndex(x => x.UserId).IsUnique(); 
        builder.Ignore(x => x.Items);

        // Items: private List<CartItem> _items
        builder.OwnsMany<CartItem>("_items", items =>
        {
            items.ToTable("cart_items");
            items.WithOwner().HasForeignKey("cart_id");

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

            
            items.HasIndex("cart_id", nameof(CartItem.ProductId)).IsUnique();
        });


        builder.Navigation("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
