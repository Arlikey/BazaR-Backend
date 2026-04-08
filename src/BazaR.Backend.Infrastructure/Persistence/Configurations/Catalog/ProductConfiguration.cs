using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");

        b.HasKey(x => x.Id);

        // =========================
        // Id
        // =========================
        b.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ProductId(value));

        // =========================
        // Ownership
        // =========================
        b.Property(x => x.OwnerSellerId)
            .HasColumnName("owner_seller_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new SellerId(value));

        // =========================
        // Basic fields
        // =========================
        b.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(5000);

        b.Property(x => x.CategoryId)
            .HasColumnName("category_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));

        // BrandId? (nullable VO)
        b.Property(x => x.BrandId)
            .HasColumnName("brand_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new BrandId(value.Value) : null);

        // Slug? (nullable VO)
        b.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .HasConversion(
                slug => slug == null ? null : slug.Value,
                value => value == null ? null : ProductSlug.Create(value).Value!);

        // VendorCode? (nullable VO)
        b.Property(x => x.VendorCode)
            .HasColumnName("vendor_code")
            .HasMaxLength(100)
            .HasConversion(
                vc => vc == null ? null : vc.Value,
                value => value == null ? null : VendorCode.Create(value).Value!);

        // Barcode? (nullable VO)
        b.Property(x => x.Barcode)
            .HasColumnName("barcode")
            .HasMaxLength(50)
            .HasConversion(
                bc => bc == null ? null : bc.Value,
                value => value == null ? null : ProductBarcode.Create(value).Value!);

        // =========================
        // Status + moderation info
        // =========================
        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        b.Property(x => x.HiddenAt)
            .HasColumnName("hidden_at");

        b.Property(x => x.HiddenBy)
            .HasColumnName("hidden_by");

        b.Property(x => x.HiddenReason)
            .HasColumnName("hidden_reason")
            .HasMaxLength(500);

        // =========================
        // Audit
        // =========================
        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // =========================
        // Indexes
        // =========================
        b.HasIndex(x => x.OwnerSellerId)
            .HasDatabaseName("ix_products_owner_seller_id");

        b.HasIndex(x => x.CategoryId)
            .HasDatabaseName("ix_products_category_id");

        b.HasIndex(x => x.Status)
            .HasDatabaseName("ix_products_status");

        b.HasIndex(x => x.Barcode)
            .HasDatabaseName("ix_products_barcode");

        // Уникальность внутри продавца
        b.HasIndex(x => new { x.OwnerSellerId, x.Slug })
            .IsUnique()
            .HasFilter("slug IS NOT NULL")
            .HasDatabaseName("ux_products_owner_slug");

        b.HasIndex(x => new { x.OwnerSellerId, x.VendorCode })
            .IsUnique()
            .HasFilter("vendor_code IS NOT NULL")
            .HasDatabaseName("ux_products_owner_vendor_code");

        // =========================
        // AttributeValues (collection)
        // =========================
        b.HasMany(x => x.AttributeValues)
    .WithOne()
    .HasForeignKey(av => av.ProductId)  
    .OnDelete(DeleteBehavior.Cascade);

        var attrNav = b.Metadata.FindNavigation(nameof(Product.AttributeValues))!;
        attrNav.SetPropertyAccessMode(PropertyAccessMode.Field);
        attrNav.SetField("_attributeValues");

        // =========================
        // Images (collection)
        // =========================
        b.HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey("product_id")
            .OnDelete(DeleteBehavior.Cascade);

        var imagesNav = b.Metadata.FindNavigation(nameof(Product.Images))!;
        imagesNav.SetPropertyAccessMode(PropertyAccessMode.Field);
        imagesNav.SetField("_images");

        // =========================
        // Ignore domain events
        // =========================
        b.Ignore("DomainEvents");

        // =========================
        // Constraints
        // =========================
        b.ToTable(t =>
        {
            t.HasCheckConstraint("ck_products_name_not_empty", "char_length(name) > 0");
            t.HasCheckConstraint("ck_products_status_valid", "status IN (0,1,2,3)");
            t.HasCheckConstraint(
                "ck_products_hidden_fields",
                "(status <> 3) OR (hidden_at IS NOT NULL AND hidden_by IS NOT NULL)");
        });
    }
} 