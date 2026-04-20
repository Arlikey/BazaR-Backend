using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.ViewedProducts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class ViewedProductConfiguration : IEntityTypeConfiguration<ViewedProduct>
{
    public void Configure(EntityTypeBuilder<ViewedProduct> builder)
    {
        builder.ToTable("viewed_products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .HasConversion(
                id => id.Value,
                value => new ProductId(value))
            .IsRequired();

        builder.Property(x => x.ViewedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.ProductId })
            .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.ViewedAtUtc });

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}