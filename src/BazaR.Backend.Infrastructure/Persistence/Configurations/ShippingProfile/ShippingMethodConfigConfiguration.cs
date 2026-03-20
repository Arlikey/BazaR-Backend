using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.ShippingProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class ShippingMethodConfigConfiguration : IEntityTypeConfiguration<ShippingMethodConfig>
{
    public void Configure(EntityTypeBuilder<ShippingMethodConfig> builder)
    {
        builder.ToTable("shipping_profile_methods");

        ConfigureKeys(builder);
        ConfigureProperties(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<ShippingMethodConfig> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ShippingMethodConfigId(value));

        builder.Property<ShippingProfileId>("ShippingProfileId")
            .HasColumnName("shipping_profile_id")
            .HasConversion(
                id => id.Value,
                value => new ShippingProfileId(value))
            .IsRequired();
    }

    private static void ConfigureProperties(EntityTypeBuilder<ShippingMethodConfig> builder)
    {
        builder.Property(x => x.MethodType)
            .HasColumnName("method_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        builder.Property(x => x.BaseFee)
            .HasColumnName("base_fee")
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasColumnName("currency")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.FreeShippingFromAmount)
            .HasColumnName("free_shipping_from_amount")
            .HasPrecision(18, 2);

        builder.Property(x => x.AllowCashOnDelivery)
            .HasColumnName("allow_cash_on_delivery")
            .IsRequired();

        builder.Property(x => x.RequiresCity)
            .HasColumnName("requires_city")
            .IsRequired();

        builder.Property(x => x.RequiresPickupPoint)
            .HasColumnName("requires_pickup_point")
            .IsRequired();

        builder.Property(x => x.RequiresStreetAddress)
            .HasColumnName("requires_street_address")
            .IsRequired();

        builder.Property(x => x.EstimatedDaysMin)
            .HasColumnName("estimated_days_min");

        builder.Property(x => x.EstimatedDaysMax)
            .HasColumnName("estimated_days_max");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<ShippingMethodConfig> builder)
    {
        builder.HasIndex("ShippingProfileId")
            .HasDatabaseName("ix_shipping_profile_methods_profile_id");

        builder.HasIndex(x => x.MethodType)
            .HasDatabaseName("ix_shipping_profile_methods_method_type");

        builder.HasIndex("ShippingProfileId", nameof(ShippingMethodConfig.MethodType))
            .IsUnique()
            .HasDatabaseName("ux_shipping_profile_methods_profile_id_method_type");
    }
}