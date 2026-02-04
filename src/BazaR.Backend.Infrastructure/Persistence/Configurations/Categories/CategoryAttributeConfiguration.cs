using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
{
    public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
    {
        builder.ToTable("category_attributes");

        // ======================
        // Key (CategoryAttributeId)
        // ======================
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CategoryAttributeId(value)
            );

        // ======================
        // FK to Category (shadow property category_id)
        // ======================
        builder.Property<CategoryId>("category_id")
            .HasColumnName("category_id")
            .HasConversion(
                id => id.Value,
                value => new CategoryId(value)
            )
            .IsRequired();

        // ======================
        // AttributeId (VO)
        // ======================
        builder.Property(x => x.AttributeId)
            .HasColumnName("attribute_id")
            .HasConversion(
                id => id.Value,
                value => new AttributeId(value)
            )
            .IsRequired();

        // ======================
        // Rules
        // ======================
        builder.Property(x => x.IsRequired)
            .HasColumnName("is_required")
            .IsRequired();

        builder.Property(x => x.IsFilterable)
            .HasColumnName("is_filterable")
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .HasColumnName("sort_order")
            .IsRequired();

        
        builder.Property(x => x.SectionName)
            .HasColumnName("section_name")
            .HasMaxLength(100);

        builder.Property(x => x.SectionOrder)
            .HasColumnName("section_order");

        // ======================
        // Indexes + Unique
        // ======================
        builder.HasIndex("category_id")
            .HasDatabaseName("ix_category_attributes_category_id");

        builder.HasIndex(x => x.AttributeId)
            .HasDatabaseName("ix_category_attributes_attribute_id");

        // Один и тот же атрибут нельзя назначить категории дважды
        builder.HasIndex("category_id", nameof(CategoryAttribute.AttributeId))
    .IsUnique()
    .HasDatabaseName("ux_category_attributes_category_attribute");


       
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_category_attributes_sort_order_non_negative", "sort_order >= 0");
            t.HasCheckConstraint("ck_category_attributes_section_order_non_negative", "section_order IS NULL OR section_order >= 0");
        });
    }
}
