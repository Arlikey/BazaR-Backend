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

        ConfigureKeys(builder);
        ConfigureEnums(builder);
        ConfigurePrimitiveProperties(builder);
        ConfigureRecipient(builder);
        ConfigureDestination(builder);
        ConfigureMoney(builder);
        ConfigureDates(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<Shipping> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ShippingId(value));

        builder.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .HasConversion(
                id => id.Value,
                value => new OrderId(value))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .HasConversion(
                id => id.Value,
                value => new SellerId(value))
            .IsRequired();
    }

    private static void ConfigureEnums(EntityTypeBuilder<Shipping> builder)
    {
        builder.Property(x => x.MethodType)
            .HasColumnName("method_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }

    private static void ConfigurePrimitiveProperties(EntityTypeBuilder<Shipping> builder)
    {
        builder.Property(x => x.CashOnDeliveryAllowed)
            .HasColumnName("cash_on_delivery_allowed")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasColumnName("comment")
            .HasColumnType("text");

        builder.Property(x => x.Carrier)
            .HasColumnName("carrier")
            .HasMaxLength(200);

        builder.Property(x => x.TrackingNumber)
            .HasColumnName("tracking_number")
            .HasMaxLength(200);
    }

    private static void ConfigureRecipient(EntityTypeBuilder<Shipping> builder)
    {
        builder.OwnsOne(x => x.Recipient, recipient =>
        {
            recipient.WithOwner();

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
                .HasMaxLength(200);

            recipient.Ignore(x => x.FullName);
        });

        builder.Navigation(x => x.Recipient)
            .IsRequired();
    }

    private static void ConfigureDestination(EntityTypeBuilder<Shipping> builder)
    {
        builder.OwnsOne(x => x.Destination, destination =>
        {
            destination.WithOwner();

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
                .HasMaxLength(100)
                .IsRequired();

            destination.Property(x => x.Street)
                .HasColumnName("destination_street")
                .HasMaxLength(200);

            destination.Property(x => x.House)
                .HasColumnName("destination_house")
                .HasMaxLength(50);

            destination.Property(x => x.Apartment)
                .HasColumnName("destination_apartment")
                .HasMaxLength(50);

            destination.Property(x => x.PostalCode)
                .HasColumnName("destination_postal_code")
                .HasMaxLength(50);

            destination.Property(x => x.PickupPointCode)
                .HasColumnName("destination_pickup_point_code")
                .HasMaxLength(100);

            destination.Property(x => x.PickupPointName)
                .HasColumnName("destination_pickup_point_name")
                .HasMaxLength(200);
        });

        builder.Navigation(x => x.Destination)
            .IsRequired();
    }

    private static void ConfigureMoney(EntityTypeBuilder<Shipping> builder)
    {
        builder.OwnsOne(x => x.Cost, money =>
        {
            money.WithOwner();

            money.Property(x => x.Amount)
                .HasColumnName("cost_amount_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("cost_amount_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.Cost)
            .IsRequired();
    }

    private static void ConfigureDates(EntityTypeBuilder<Shipping> builder)
    {
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.PreparingAtUtc)
            .HasColumnName("preparing_at_utc");

        builder.Property(x => x.ReadyToShipAtUtc)
            .HasColumnName("ready_to_ship_at_utc");

        builder.Property(x => x.ShippedAtUtc)
            .HasColumnName("shipped_at_utc");

        builder.Property(x => x.DeliveredAtUtc)
            .HasColumnName("delivered_at_utc");

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(x => x.ReturnedAtUtc)
            .HasColumnName("returned_at_utc");
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Shipping> builder)
    {
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("ix_shippings_order_id");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_shippings_user_id");

        builder.HasIndex(x => x.SellerId)
            .HasDatabaseName("ix_shippings_seller_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_shippings_status");

        builder.HasIndex(x => x.MethodType)
            .HasDatabaseName("ix_shippings_method_type");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_shippings_created_at_utc");

        builder.HasIndex(x => x.ShippedAtUtc)
            .HasDatabaseName("ix_shippings_shipped_at_utc");

        builder.HasIndex(x => x.DeliveredAtUtc)
            .HasDatabaseName("ix_shippings_delivered_at_utc");

        builder.HasIndex(x => x.TrackingNumber)
            .HasDatabaseName("ix_shippings_tracking_number");
    }
}