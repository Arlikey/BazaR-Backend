using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ProductId(value)
            );

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(5000);

        
        builder.Property(x => x.CategoryId)
            .HasColumnName("category_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value)
            );

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.BrandId)
            .HasColumnName("brand_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new BrandId(value.Value) : (BrandId?)null
            );

        builder.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .HasConversion(
                slug => slug == null ? null : slug.Value,
                value => value == null ? null : ProductSlug.Create(value).Value!
            );

        builder.Property(x => x.VendorCode)
            .HasColumnName("vendor_code")
            .HasMaxLength(100)
            .HasConversion(
                vc => vc == null ? null : vc.Value,
                value => value == null ? null : VendorCode.Create(value).Value!
            );

        builder.HasIndex(x => x.CategoryId)
            .HasDatabaseName("ix_products_category_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_products_status");

        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("slug IS NOT NULL")
            .HasDatabaseName("ux_products_slug");

        builder.HasIndex(x => x.VendorCode)
            .IsUnique()
            .HasFilter("vendor_code IS NOT NULL")
            .HasDatabaseName("ux_products_vendor_code");


        builder.HasMany(x => x.AttributeValues)
      .WithOne()
      .HasForeignKey("product_id")
      .OnDelete(DeleteBehavior.Cascade);

        
        builder.Metadata.FindNavigation(nameof(Product.AttributeValues))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore("DomainEvents");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_products_name_not_empty", "char_length(name) > 0");
            t.HasCheckConstraint("ck_products_status_valid", "status IN (0,1,2)");
        });
    }
}
