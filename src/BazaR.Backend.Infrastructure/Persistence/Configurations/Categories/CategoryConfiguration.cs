using BazaR.Backend.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        // ======================
        // Key (CategoryId)
        // ======================
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value));

        // ======================
        // Fields
        // ======================
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasColumnName("slug")
            .HasMaxLength(200)
            .HasConversion(
                slug => slug == null ? null : slug.Value,
                value => string.IsNullOrWhiteSpace(value)
                    ? null
                    : CategorySlug.Create(value).Value);

        // ParentCategoryId? (nullable VO) -> Guid?
        builder.Property(x => x.ParentCategoryId)
            .HasColumnName("parent_category_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new CategoryId(value.Value) : (CategoryId?)null
            );

        // ======================
        // Owned Image (optional)
        // ======================
        builder.OwnsOne(x => x.Image, img =>
        {
            img.Property(x => x.Url)
                .HasColumnName("image_url")
                .HasMaxLength(2000);

            img.Property(x => x.StorageKey)
                .HasColumnName("image_storage_key")
                .HasMaxLength(1024);

            img.Property(x => x.ContentType)
                .HasColumnName("image_content_type")
                .HasMaxLength(100);

            img.Property(x => x.SizeBytes)
                .HasColumnName("image_size_bytes");

            img.HasIndex(x => x.StorageKey)
                .HasDatabaseName("ix_categories_image_storage_key");
        });

        builder.Navigation(x => x.Image)
            .IsRequired(false)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ======================
        // CategoryAttributes (1:N)
        // ======================
        builder.HasMany(x => x.Attributes)
            .WithOne()
            .HasForeignKey("category_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ======================
        // Indexes
        // ======================
        builder.HasIndex(x => x.ParentCategoryId)
            .HasDatabaseName("ix_categories_parent_id");

        builder.HasIndex(x => new { x.ParentCategoryId, x.SortOrder })
            .HasDatabaseName("ix_categories_parent_sort");

        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasDatabaseName("ix_categories_slug");

        // ======================
        // Ignore domain events
        // ======================
        builder.Ignore("DomainEvents");

        // ======================
        // Check constraints
        // ======================
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_categories_sort_order_non_negative", "sort_order >= 0");
            t.HasCheckConstraint("ck_categories_name_not_empty", "char_length(name) > 0");

            t.HasCheckConstraint(
                "ck_categories_image_size_positive_or_null",
                "image_size_bytes IS NULL OR image_size_bytes > 0");

            t.HasCheckConstraint(
                "ck_categories_image_pair",
                "(image_url IS NULL AND image_storage_key IS NULL) OR (image_url IS NOT NULL AND image_storage_key IS NOT NULL)");

            t.HasCheckConstraint(
                "ck_categories_slug_not_empty",
                "slug IS NULL OR char_length(slug) > 0");
        });
    }
}