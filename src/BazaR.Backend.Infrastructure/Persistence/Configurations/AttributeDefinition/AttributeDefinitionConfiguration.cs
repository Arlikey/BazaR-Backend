using BazaR.Backend.Domain.Catalog.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class AttributeDefinitionConfiguration : IEntityTypeConfiguration<AttributeDefinition>
{
    public void Configure(EntityTypeBuilder<AttributeDefinition> builder)
    {
        builder.ToTable("attribute_definitions");

        // ======================
        // Key (AttributeId)
        // ======================
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new AttributeId(value)
            );

        // ======================
        // Fields
        // ======================
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique()
            .HasDatabaseName("ux_attribute_definitions_code");

        builder.Property(x => x.ValueType)
            .HasColumnName("value_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Unit)
            .HasColumnName("unit")
            .HasMaxLength(20);

        builder.Property(x => x.IsSystem)
            .HasColumnName("is_system")
            .IsRequired();

       
        builder.Ignore("DomainEvents");

        // ======================
        // Options (owned collection)
        // ======================
        builder.Navigation(x => x.Options)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.Options, opt =>
        {
            opt.ToTable("attribute_options");

            opt.WithOwner()
                .HasForeignKey("attribute_definition_id");

            // Составной ключ (attribute_definition_id + id)
            opt.HasKey("attribute_definition_id", "Id");

            // Или отдельный первичный ключ
            opt.Property(o => o.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            opt.Property(o => o.Value)
                .HasColumnName("value") 
                .HasMaxLength(200)
                .IsRequired();

            opt.Property(o => o.SortOrder)
                .HasColumnName("sort_order")
                .IsRequired();

          
            opt.HasIndex("attribute_definition_id", nameof(AttributeOption.Value)) 
                .IsUnique()
                .HasDatabaseName("ux_attribute_options_definition_value");

            opt.ToTable(t =>
            {
                t.HasCheckConstraint("ck_attribute_options_sort_order_non_negative", "sort_order >= 0");
                t.HasCheckConstraint("ck_attribute_options_value_not_empty", "char_length(value) > 0");
            });
        });

        // ======================
        // Check constraints
        // ======================
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_attribute_definitions_name_not_empty", "char_length(name) > 0");
            t.HasCheckConstraint("ck_attribute_definitions_code_not_empty", "char_length(code) > 0");
        });
    }
}