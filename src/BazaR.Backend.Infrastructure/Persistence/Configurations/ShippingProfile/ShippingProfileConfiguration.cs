using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.ShippingProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class ShippingProfileConfiguration : IEntityTypeConfiguration<ShippingProfile>
{
    public void Configure(EntityTypeBuilder<ShippingProfile> builder)
    {
        builder.ToTable("shipping_profiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ShippingProfileId(value));

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .HasConversion(
                id => id.Value,
                value => new SellerId(value))
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.HasIndex(x => x.SellerId)
            .HasDatabaseName("ix_shipping_profiles_seller_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_shipping_profiles_status");

        builder.HasMany(x => x.Methods)
            .WithOne()
            .HasForeignKey("ShippingProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(ShippingProfile.Methods))!
            .SetField("_methods");

        builder.Metadata
            .FindNavigation(nameof(ShippingProfile.Methods))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}