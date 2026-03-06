using BazaR.Backend.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        b.ToTable("product_images");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

      
        b.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .HasConversion(
                id => id.Value,
                value => new ProductId(value))
            .IsRequired();

        b.Property(x => x.Url)
            .HasColumnName("url")
            .HasMaxLength(2048)
            .IsRequired();

        b.Property(x => x.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(512)
            .IsRequired();

        b.Property(x => x.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100)
            .IsRequired();

        b.Property(x => x.SizeBytes)
            .HasColumnName("size_bytes")
            .IsRequired();

        b.Property(x => x.IsMain)
            .HasColumnName("is_main")
            .IsRequired();

        b.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Индексы (теперь через свойства)
        b.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_product_images_product_id");

        b.HasIndex(x => new { x.ProductId, x.IsMain })
            .HasDatabaseName("ix_product_images_product_main")
            .HasFilter("is_main = true");
    }
}