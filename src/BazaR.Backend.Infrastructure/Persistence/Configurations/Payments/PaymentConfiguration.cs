using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Sellers; // <-- добавить using
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        ConfigureKeys(builder);
        ConfigureEnums(builder);
        ConfigurePrimitiveProperties(builder);
        ConfigureMoney(builder);
        ConfigureDates(builder);
        ConfigureIgnoredProperties(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new PaymentId(value));

        builder.Property(x => x.OrderId)
            .HasColumnName("order_id")
            .HasConversion(
                id => id.Value,
                value => new OrderId(value))
            .IsRequired();

       
        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .HasConversion(
                id => id.Value,
                value => new SellerId(value))
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();
    }

    private static void ConfigureEnums(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.Provider)
            .HasColumnName("provider")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Method)
            .HasColumnName("method")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

     
        builder.Property(x => x.RequestedLiqPayPayType)
            .HasColumnName("requested_liqpay_pay_type")
            .HasConversion<string?>()
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(x => x.ActualLiqPayPayType)
            .HasColumnName("actual_liqpay_pay_type")
            .HasConversion<string?>()
            .HasMaxLength(20)
            .IsRequired(false);
    }

    private static void ConfigurePrimitiveProperties(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.MerchantOrderReference)
            .HasColumnName("merchant_order_reference")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ExternalPaymentId)
            .HasColumnName("external_payment_id")
            .HasMaxLength(200);

        builder.Property(x => x.ExternalOrderReference)
            .HasColumnName("external_order_reference")
            .HasMaxLength(200);

        builder.Property(x => x.ExternalSessionId)
            .HasColumnName("external_session_id")
            .HasMaxLength(200);

      
        builder.Property(x => x.ExternalTransactionId)
            .HasColumnName("external_transaction_id")
            .HasMaxLength(200);

        builder.Property(x => x.ExternalStatus)
            .HasColumnName("external_status")
            .HasMaxLength(100);

        builder.Property(x => x.CheckoutActionUrl)
            .HasColumnName("checkout_action_url")
            .HasColumnType("text");

        builder.Property(x => x.CheckoutData)
            .HasColumnName("checkout_data")
            .HasColumnType("text");

        builder.Property(x => x.CheckoutSignature)
            .HasColumnName("checkout_signature")
            .HasColumnType("text");

       
        builder.Property(x => x.CallbackData)
            .HasColumnName("callback_data")
            .HasColumnType("text");

        builder.Property(x => x.CallbackSignature)
            .HasColumnName("callback_signature")
            .HasColumnType("text");

        
        builder.Property(x => x.CardMask)
            .HasColumnName("card_mask")
            .HasMaxLength(50);

        builder.Property(x => x.CardBank)
            .HasColumnName("card_bank")
            .HasMaxLength(100);

        builder.Property(x => x.CardType)
            .HasColumnName("card_type")
            .HasMaxLength(50);

       
        builder.Property(x => x.ProviderAmount)
            .HasColumnName("provider_amount")
            .HasPrecision(18, 2);

        builder.Property(x => x.ProviderCurrency)
            .HasColumnName("provider_currency")
            .HasMaxLength(10);

        builder.Property(x => x.FailureCode)
            .HasColumnName("failure_code")
            .HasMaxLength(100);

        builder.Property(x => x.FailureMessage)
            .HasColumnName("failure_message")
            .HasColumnType("text");
    }

    private static void ConfigureMoney(EntityTypeBuilder<Payment> builder)
    {
        builder.OwnsOne(x => x.Amount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("amount_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("amount_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.Amount)
            .IsRequired();

        builder.OwnsOne(x => x.RefundedAmount, money =>
        {
            money.Property(x => x.Amount)
                .HasColumnName("refunded_amount_value")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(x => x.Currency)
                .HasColumnName("refunded_amount_currency")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.Navigation(x => x.RefundedAmount)
            .IsRequired();
    }

    private static void ConfigureDates(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.CheckoutStartedAtUtc)
            .HasColumnName("checkout_started_at_utc");

        builder.Property(x => x.CallbackReceivedAtUtc)   
            .HasColumnName("callback_received_at_utc");

        builder.Property(x => x.AuthorizedAtUtc)
            .HasColumnName("authorized_at_utc");

        builder.Property(x => x.PaidAtUtc)
            .HasColumnName("paid_at_utc");

        builder.Property(x => x.FailedAtUtc)
            .HasColumnName("failed_at_utc");

        builder.Property(x => x.CancelledAtUtc)
            .HasColumnName("cancelled_at_utc");

        builder.Property(x => x.RefundedAtUtc)
            .HasColumnName("refunded_at_utc");

        builder.Property(x => x.LastProviderSyncAtUtc)   
            .HasColumnName("last_provider_sync_at_utc");
    }

    private static void ConfigureIgnoredProperties(EntityTypeBuilder<Payment> builder)
    {
        builder.Ignore(x => x.IsPaid);
        builder.Ignore(x => x.IsFinal);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<Payment> builder)
    {
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("ix_payments_order_id");

        builder.HasIndex(x => x.UserId)
            .HasDatabaseName("ix_payments_user_id");

        builder.HasIndex(x => x.SellerId) 
            .HasDatabaseName("ix_payments_seller_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_payments_status");

        builder.HasIndex(x => x.Provider)
            .HasDatabaseName("ix_payments_provider");

        builder.HasIndex(x => x.Method)
            .HasDatabaseName("ix_payments_method");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_payments_created_at_utc");

        builder.HasIndex(x => x.PaidAtUtc)
            .HasDatabaseName("ix_payments_paid_at_utc");

        builder.HasIndex(x => x.ExternalPaymentId)
            .HasDatabaseName("ix_payments_external_payment_id");

        builder.HasIndex(x => x.ExternalTransactionId)   
            .HasDatabaseName("ix_payments_external_transaction_id");

        builder.HasIndex(x => x.ExternalOrderReference)
            .HasDatabaseName("ix_payments_external_order_reference");

        builder.HasIndex(x => x.ExternalSessionId)
            .HasDatabaseName("ix_payments_external_session_id");

        builder.HasIndex(x => x.MerchantOrderReference)
            .IsUnique()
            .HasDatabaseName("ux_payments_merchant_order_reference");
    }
}