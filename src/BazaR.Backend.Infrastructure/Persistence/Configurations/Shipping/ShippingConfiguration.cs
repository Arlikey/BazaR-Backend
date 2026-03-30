using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
{
    public void Configure(EntityTypeBuilder<Shipping> builder)
    {
        builder.ToTable("shippings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ShippingId(value));

        builder.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new OrderId(value));

        builder.Property(x => x.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new UserId(value));

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new SellerId(value));

        builder.Property(x => x.Method)
            .HasColumnName("method")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.SettlementMode)
            .HasColumnName("settlement_mode")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.TrackingNumber)
            .HasColumnName("tracking_number")
            .HasMaxLength(128);

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.DispatchedAtUtc)
            .HasColumnName("dispatched_at_utc");

        builder.Property(x => x.ReadyForPickupAtUtc)
            .HasColumnName("ready_for_pickup_at_utc");

        builder.Property(x => x.DeliveredAtUtc)
            .HasColumnName("delivered_at_utc");

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        ConfigureSender(builder);
        ConfigureRecipient(builder);
        ConfigureDestination(builder);
        ConfigureParcels(builder);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.SellerId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Method);
        builder.HasIndex(x => x.TrackingNumber);

        builder.Navigation(x => x.Recipient)
            .IsRequired();

        builder.Navigation(x => x.Destination)
            .IsRequired();
    }

    private static void ConfigureSender(EntityTypeBuilder<Shipping> builder)
    {
        builder.OwnsOne(x => x.Sender, sender =>
        {
            sender.Property(x => x.Name)
                .HasColumnName("sender_name")
                .HasMaxLength(200);

            sender.Property(x => x.Phone)
                .HasColumnName("sender_phone")
                .HasMaxLength(50);

            sender.Property(x => x.CountryCode)
                .HasColumnName("sender_country_code")
                .HasMaxLength(16);

            sender.Property(x => x.PickupPointCode)
                .HasColumnName("sender_pickup_point_code")
                .HasMaxLength(128);

            sender.Property(x => x.PickupPointName)
                .HasColumnName("sender_pickup_point_name")
                .HasMaxLength(256);
        });
    }

    private static void ConfigureRecipient(EntityTypeBuilder<Shipping> builder)
    {
        builder.OwnsOne(x => x.Recipient, recipient =>
        {
            recipient.Property(x => x.FirstName)
                .HasColumnName("recipient_first_name")
                .HasMaxLength(100)
                .IsRequired();

            recipient.Property(x => x.LastName)
                .HasColumnName("recipient_last_name")
                .HasMaxLength(100)
                .IsRequired();

            recipient.Property(x => x.Phone)
                .HasColumnName("recipient_phone")
                .HasMaxLength(50)
                .IsRequired();

            recipient.Property(x => x.Email)
                .HasColumnName("recipient_email")
                .HasMaxLength(256);
        });
    }

    private static void ConfigureDestination(EntityTypeBuilder<Shipping> builder)
    {
        builder.OwnsOne(x => x.Destination, destination =>
        {
            destination.Property(x => x.Country)
                .HasColumnName("destination_country")
                .HasMaxLength(100)
                .IsRequired();

            destination.Property(x => x.Region)
                .HasColumnName("destination_region")
                .HasMaxLength(100)
                .IsRequired();

            destination.Property(x => x.City)
                .HasColumnName("destination_city")
                .HasMaxLength(150)
                .IsRequired();

            destination.Property(x => x.Street)
                .HasColumnName("destination_street")
                .HasMaxLength(150);

            destination.Property(x => x.House)
                .HasColumnName("destination_house")
                .HasMaxLength(50);

            destination.Property(x => x.Apartment)
                .HasColumnName("destination_apartment")
                .HasMaxLength(50);

            destination.Property(x => x.PostalCode)
                .HasColumnName("destination_postal_code")
                .HasMaxLength(32);

            destination.Property(x => x.PickupPointCode)
                .HasColumnName("destination_pickup_point_code")
                .HasMaxLength(128);

            destination.Property(x => x.PickupPointName)
                .HasColumnName("destination_pickup_point_name")
                .HasMaxLength(256);
        });
    }

    private static void ConfigureParcels(EntityTypeBuilder<Shipping> builder)
    {
        builder.Metadata
            .FindNavigation(nameof(Shipping.Parcels))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(x => x.Parcels, parcel =>
        {
            parcel.ToTable("shipping_parcels");

            parcel.WithOwner()
                .HasForeignKey("shipping_id");

            parcel.Property<Guid>("id")
                .HasColumnName("id");

            parcel.HasKey("id");

            parcel.Property(x => x.RowNumber)
                .HasColumnName("row_number")
                .IsRequired();

            parcel.Property(x => x.CargoCategory)
                .HasColumnName("cargo_category")
                .HasMaxLength(64)
                .IsRequired();

            parcel.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired();

            parcel.Property(x => x.InsuranceCost)
                .HasColumnName("insurance_cost")
                .HasPrecision(18, 2)
                .IsRequired();

            parcel.Property(x => x.Width)
                .HasColumnName("width")
                .HasPrecision(18, 3)
                .IsRequired();

            parcel.Property(x => x.Length)
                .HasColumnName("length")
                .HasPrecision(18, 3)
                .IsRequired();

            parcel.Property(x => x.Height)
                .HasColumnName("height")
                .HasPrecision(18, 3)
                .IsRequired();

            parcel.Property(x => x.ActualWeight)
                .HasColumnName("actual_weight")
                .HasPrecision(18, 3)
                .IsRequired();

            parcel.Property(x => x.VolumetricWeight)
                .HasColumnName("volumetric_weight")
                .HasPrecision(18, 3)
                .IsRequired();

            parcel.HasIndex("shipping_id", nameof(ShippingParcel.RowNumber))
                .IsUnique();
        });
    }
}