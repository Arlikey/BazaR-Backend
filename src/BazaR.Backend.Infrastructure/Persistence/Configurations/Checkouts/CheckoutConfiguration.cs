using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class CheckoutConfiguration : IEntityTypeConfiguration<Checkout>
{
    public void Configure(EntityTypeBuilder<Checkout> builder)
    {
        builder.ToTable("checkouts");

        ConfigureKeys(builder);
        ConfigureStatus(builder);
        ConfigureTotals(builder);
        ConfigureDates(builder);
        ConfigureLines(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<Checkout> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new CheckoutId(value));

        builder.Property(x => x.CartId)
            .HasColumnName("cart_id")
            .HasConversion(
                id => id.Value,
                value => new CartId(value))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();
    }

    private static void ConfigureStatus(EntityTypeBuilder<Checkout> builder)
    {
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }

    private static void ConfigureTotals(EntityTypeBuilder<Checkout> builder)
    {
        builder.OwnsOne(x => x.ItemsSubtotal, money =>
        {
            money.WithOwner();

            money.Property(x => x.Amount)
                .HasColumnName("items_subtotal_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("items_subtotal_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.ItemsSubtotal)
            .IsRequired();

        builder.OwnsOne(x => x.ShippingTotal, money =>
        {
            money.WithOwner();

            money.Property(x => x.Amount)
                .HasColumnName("shipping_total_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("shipping_total_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.ShippingTotal)
            .IsRequired();

        builder.OwnsOne(x => x.GrandTotal, money =>
        {
            money.WithOwner();

            money.Property(x => x.Amount)
                .HasColumnName("grand_total_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("grand_total_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.GrandTotal)
            .IsRequired();
    }

    private static void ConfigureDates(EntityTypeBuilder<Checkout> builder)
    {
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.SubmittedAtUtc)
            .HasColumnName("submitted_at_utc");

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");
    }

    private static void ConfigureLines(EntityTypeBuilder<Checkout> builder)
    {
        builder.OwnsMany(x => x.Lines, line =>
        {
            line.ToTable("checkout_lines");

            line.WithOwner()
                .HasForeignKey("CheckoutId");

            line.Property<CheckoutId>("CheckoutId")
                .HasColumnName("checkout_id")
                .HasConversion(
                    id => id.Value,
                    value => new CheckoutId(value))
                .IsRequired();

            line.HasKey("CheckoutId", nameof(CheckoutLine.Id));

            line.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => new CheckoutLineId(value));

            line.Property(x => x.OfferId)
                .HasColumnName("offer_id")
                .HasConversion(
                    id => id.Value,
                    value => new OfferId(value))
                .IsRequired();

            line.Property(x => x.ProductId)
                .HasColumnName("product_id")
                .HasConversion(
                    id => id.Value,
                    value => new ProductId(value))
                .IsRequired();

            line.Property(x => x.SellerId)
                .HasColumnName("seller_id")
                .HasConversion(
                    id => id.Value,
                    value => new SellerId(value))
                .IsRequired();

            line.Property(x => x.ProductTitle)
                .HasColumnName("product_title")
                .HasMaxLength(500)
                .IsRequired();

            line.Property(x => x.Sku)
                .HasColumnName("sku")
                .HasMaxLength(100)
                .IsRequired();

            line.Property(x => x.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            line.OwnsOne(x => x.UnitPrice, money =>
            {
                money.WithOwner();

                money.Property(x => x.Amount)
                    .HasColumnName("unit_price_value")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(x => x.Currency)
                    .HasColumnName("unit_price_currency")
                    .HasMaxLength(10)
                    .IsRequired();
            });

            line.Navigation(x => x.UnitPrice)
                .IsRequired();

            line.Ignore(x => x.LineTotal);

            line.OwnsOne(x => x.Recipient, recipient =>
            {
                recipient.WithOwner();

                recipient.Property(x => x.FirstName)
                    .HasColumnName("recipient_first_name")
                    .HasMaxLength(100);

                recipient.Property(x => x.LastName)
                    .HasColumnName("recipient_last_name")
                    .HasMaxLength(100);

                recipient.Property(x => x.Phone)
                    .HasColumnName("recipient_phone")
                    .HasMaxLength(50);

                recipient.Property(x => x.Email)
                    .HasColumnName("recipient_email")
                    .HasMaxLength(200);

                recipient.Property(x => x.IsCustomerRecipient)
                    .HasColumnName("recipient_is_customer_recipient");

                recipient.Ignore(x => x.FullName);
            });

            line.OwnsOne(x => x.Payment, payment =>
            {
                payment.WithOwner();

                payment.Property(x => x.Method)
                    .HasColumnName("payment_method")
                    .HasConversion<string>()
                    .HasMaxLength(50);

                payment.Property(x => x.Provider)
                    .HasColumnName("payment_provider")
                    .HasMaxLength(100);

                payment.Property(x => x.RequiresOnlineAuthorization)
                    .HasColumnName("payment_requires_online_authorization");
            });

            line.OwnsOne(x => x.Shipping, shipping =>
            {
                shipping.WithOwner();

                shipping.Property(x => x.MethodType)
                    .HasColumnName("shipping_method_type")
                    .HasConversion<string>()
                    .HasMaxLength(50);

                shipping.Property(x => x.Country)
                    .HasColumnName("shipping_country")
                    .HasMaxLength(100);

                shipping.Property(x => x.Region)
                    .HasColumnName("shipping_region")
                    .HasMaxLength(100);

                shipping.Property(x => x.City)
                    .HasColumnName("shipping_city")
                    .HasMaxLength(100);

                shipping.Property(x => x.Street)
                    .HasColumnName("shipping_street")
                    .HasMaxLength(200);

                shipping.Property(x => x.House)
                    .HasColumnName("shipping_house")
                    .HasMaxLength(50);

                shipping.Property(x => x.Apartment)
                    .HasColumnName("shipping_apartment")
                    .HasMaxLength(50);

                shipping.Property(x => x.PostalCode)
                    .HasColumnName("shipping_postal_code")
                    .HasMaxLength(50);

                shipping.Property(x => x.PickupPointCode)
                    .HasColumnName("shipping_pickup_point_code")
                    .HasMaxLength(100);

                shipping.Property(x => x.PickupPointName)
                    .HasColumnName("shipping_pickup_point_name")
                    .HasMaxLength(200);

                shipping.Property(x => x.Comment)
                    .HasColumnName("shipping_comment")
                    .HasColumnType("text");

                shipping.OwnsOne(x => x.Cost, money =>
                {
                    money.WithOwner();

                    money.Property(x => x.Amount)
                        .HasColumnName("shipping_cost_value")
                        .HasPrecision(18, 2)
                        .IsRequired();

                    money.Property(x => x.Currency)
                        .HasColumnName("shipping_cost_currency")
                        .HasMaxLength(10)
                        .IsRequired();
                });

                shipping.Navigation(x => x.Cost)
                    .IsRequired();
            });

            line.HasIndex("CheckoutId")
                .HasDatabaseName("ix_checkout_lines_checkout_id");

            line.HasIndex(x => x.OfferId)
                .HasDatabaseName("ix_checkout_lines_offer_id");

            line.HasIndex(x => x.ProductId)
                .HasDatabaseName("ix_checkout_lines_product_id");

            line.HasIndex(x => x.SellerId)
                .HasDatabaseName("ix_checkout_lines_seller_id");
        });

        builder.Metadata
            .FindNavigation(nameof(Checkout.Lines))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Checkout> builder)
    {
        builder.HasIndex(x => x.CartId)
            .HasDatabaseName("ix_checkouts_cart_id");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_checkouts_user_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_checkouts_status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_checkouts_created_at_utc");

        builder.HasIndex(x => x.SubmittedAtUtc)
            .HasDatabaseName("ix_checkouts_submitted_at_utc");
    }
}