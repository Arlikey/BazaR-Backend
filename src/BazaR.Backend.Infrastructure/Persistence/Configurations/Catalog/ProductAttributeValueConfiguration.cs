using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Catalog;

public sealed class ProductAttributeValueConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("product_attribute_values");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

       
        builder.Property<ProductId>("product_id")
            .HasColumnName("product_id")
            .HasConversion(
                id => id.Value,
                value => new ProductId(value))
            .IsRequired();

      
        builder.Property(x => x.AttributeId)
            .HasColumnName("attribute_id")
            .HasConversion(
                id => id.Value,
                value => new AttributeId(value))
            .IsRequired();

        builder.Property(x => x.TextValue)
            .HasColumnName("text_value")
            .HasMaxLength(2000);

        builder.Property(x => x.NumberValue)
            .HasColumnName("number_value");

        builder.Property(x => x.BoolValue)
            .HasColumnName("bool_value");

        builder.Property(x => x.OptionId)
            .HasColumnName("option_id");

        builder.HasIndex("product_id")
            .HasDatabaseName("ix_product_attribute_values_product_id");

        builder.HasIndex(x => x.AttributeId)
            .HasDatabaseName("ix_product_attribute_values_attribute_id");

    
        builder.HasIndex("product_id", nameof(ProductAttributeValue.AttributeId))
            .IsUnique()
            .HasDatabaseName("ux_product_attribute_values_product_attribute");
    }
}
