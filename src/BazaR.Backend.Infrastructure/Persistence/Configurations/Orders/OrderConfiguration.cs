using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        ConfigureKeys(builder);
        ConfigureStatus(builder);
        ConfigureCustomer(builder);
        ConfigureDelivery(builder);
        ConfigurePrimitiveProperties(builder);
        ConfigureDates(builder);
        ConfigureIgnoredProperties(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new OrderId(value));

        var buyerUserIdConverter = new ValueConverter<UserId?, Guid?>(
            id => id == null ? null : id.Value.Value,
            value => value == null ? null : new UserId(value.Value));

        builder.Property(x => x.BuyerUserId)
            .HasColumnName("buyer_user_id")
            .HasConversion(buyerUserIdConverter);
    }

    private static void ConfigureStatus(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }

    private static void ConfigureCustomer(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.Customer, customer =>
        {
            customer.WithOwner();

            customer.Property(x => x.FirstName)
                .HasColumnName("customer_first_name")
                .HasMaxLength(100)
                .IsRequired();

            customer.Property(x => x.LastName)
                .HasColumnName("customer_last_name")
                .HasMaxLength(100)
                .IsRequired();

            customer.Property(x => x.Email)
                .HasColumnName("customer_email")
                .HasMaxLength(200)
                .IsRequired();

            customer.Property(x => x.Phone)
                .HasColumnName("customer_phone")
                .HasMaxLength(50);
        });

        builder.Navigation(x => x.Customer)
            .IsRequired();
    }

    private static void ConfigureDelivery(EntityTypeBuilder<Order> builder)
    {
        builder.OwnsOne(x => x.Delivery, delivery =>
        {
            delivery.WithOwner();

            delivery.Property(x => x.Method)
                .HasColumnName("delivery_method")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            delivery.Property(x => x.City)
                .HasColumnName("delivery_city")
                .HasMaxLength(100)
                .IsRequired();

            delivery.Property(x => x.Region)
                .HasColumnName("delivery_region")
                .HasMaxLength(100);

            delivery.Property(x => x.Warehouse)
                .HasColumnName("delivery_warehouse")
                .HasMaxLength(200);

            delivery.Property(x => x.Street)
                .HasColumnName("delivery_street")
                .HasMaxLength(200);

            delivery.Property(x => x.Building)
                .HasColumnName("delivery_building")
                .HasMaxLength(50);

            delivery.Property(x => x.Apartment)
                .HasColumnName("delivery_apartment")
                .HasMaxLength(50);

            delivery.Property(x => x.PostalCode)
                .HasColumnName("delivery_postal_code")
                .HasMaxLength(50);
        });

        builder.Navigation(x => x.Delivery)
            .IsRequired();
    }

    private static void ConfigurePrimitiveProperties(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.CustomerComment)
            .HasColumnName("customer_comment")
            .HasColumnType("text");

        builder.Property(x => x.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasColumnType("text");
    }

    private static void ConfigureDates(EntityTypeBuilder<Order> builder)
    {
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.PaidAtUtc)
            .HasColumnName("paid_at_utc");

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(x => x.DeliveredAtUtc)
            .HasColumnName("delivered_at_utc");

        builder.Property(x => x.CompletedAtUtc)
            .HasColumnName("completed_at_utc");
    }

    private static void ConfigureIgnoredProperties(EntityTypeBuilder<Order> builder)
    {
        builder.Ignore(x => x.Subtotal);
        builder.Ignore(x => x.DiscountTotal);
        builder.Ignore(x => x.DeliveryFee);
        builder.Ignore(x => x.GrandTotal);
    }

    private static void ConfigureRelationships(EntityTypeBuilder<Order> builder)
    {
        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(Order.Items))!
            .SetField("_items");

        builder.Metadata
            .FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Order> builder)
    {
        builder.HasIndex(x => x.BuyerUserId)
            .HasDatabaseName("ix_orders_buyer_user_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_orders_status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_orders_created_at_utc");

        builder.HasIndex(x => x.PaidAtUtc)
            .HasDatabaseName("ix_orders_paid_at_utc");

        builder.HasIndex(x => x.DeliveredAtUtc)
            .HasDatabaseName("ix_orders_delivered_at_utc");

        builder.HasIndex(x => x.CompletedAtUtc)
            .HasDatabaseName("ix_orders_completed_at_utc");
    }
}