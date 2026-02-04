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
                value => new CategoryId(value)
            );

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

        // ParentCategoryId? (nullable VO) -> Guid?
        builder.Property(x => x.ParentCategoryId)
            .HasColumnName("parent_category_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? new CategoryId(value.Value) : (CategoryId?)null
            );

        // ======================
        // Indexes
        // ======================
        builder.HasIndex(x => x.ParentCategoryId)
            .HasDatabaseName("ix_categories_parent_id");

        
        builder.HasIndex(x => new { x.ParentCategoryId, x.SortOrder })
            .HasDatabaseName("ix_categories_parent_sort");

        

        // ======================
        // CategoryAttributes (1:N)
        // ======================
        // Привязка к приватному полю _attributes в Category
        builder.HasMany(x => x.Attributes)
    .WithOne()
    .HasForeignKey("category_id")
    .OnDelete(DeleteBehavior.Cascade);

        // Указываем использовать приватное поле для коллекции
        builder.Navigation(x => x.Attributes)
            .UsePropertyAccessMode(PropertyAccessMode.Field);


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

            });
    }
}
