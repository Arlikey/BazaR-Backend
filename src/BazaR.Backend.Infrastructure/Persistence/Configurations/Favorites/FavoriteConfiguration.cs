using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Favorites;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Favorites;

public sealed class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("favorites");

       
        builder.HasKey(x => new { x.UserId, x.ProductId });

       
        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                userId => userId.Value,
                value => new UserId(value))
            .IsRequired();

        
        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .HasConversion(
                productId => productId.Value,
                value => new ProductId(value))
            .IsRequired();

       
        builder.Property(x => x.AddedAtUtc)
            .HasColumnName("added_at_utc")
            .IsRequired();

      
        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_favorites_user_id");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_favorites_product_id");
    }
}