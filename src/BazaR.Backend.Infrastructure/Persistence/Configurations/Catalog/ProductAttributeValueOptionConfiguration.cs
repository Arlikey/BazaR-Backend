using BazaR.Backend.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductAttributeValueOptionConfiguration
    : IEntityTypeConfiguration<ProductAttributeValueOption>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValueOption> builder)
    {
        builder.ToTable("product_attribute_value_option_ids");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property<Guid>("product_attribute_value_id")
            .HasColumnName("product_attribute_value_id")
            .IsRequired();

        builder.Property(x => x.OptionId)
            .HasColumnName("option_id")
            .IsRequired();

        builder.HasIndex("product_attribute_value_id")
            .HasDatabaseName("ix_pav_option_ids_pav_id");

        builder.HasIndex(x => x.OptionId)
            .HasDatabaseName("ix_pav_option_ids_option_id");

        builder.HasIndex("product_attribute_value_id", nameof(ProductAttributeValueOption.OptionId))
            .IsUnique()
            .HasDatabaseName("ux_pav_option_ids_pav_option");
    }
}