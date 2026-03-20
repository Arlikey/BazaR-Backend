using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.PaymentProfiles;

public sealed class PaymentProfileConfiguration : IEntityTypeConfiguration<PaymentProfile>
{
    public void Configure(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.ToTable("payment_profiles");

        ConfigureKeys(builder);
        ConfigureStatus(builder);
        ConfigureValueObjects(builder);
        ConfigureDates(builder);
        ConfigureRelationships(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new PaymentProfileId(value));

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .HasConversion(
                id => id.Value,
                value => new SellerId(value))
            .IsRequired();
    }

    private static void ConfigureStatus(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();
    }

    private static void ConfigureValueObjects(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.OwnsOne(x => x.BankAccount, bank =>
        {
            bank.WithOwner();

            bank.Property(x => x.RecipientName)
                .HasColumnName("bank_recipient_name")
                .HasMaxLength(200);

            bank.Property(x => x.Iban)
                .HasColumnName("bank_iban")
                .HasMaxLength(64);

            bank.Property(x => x.BankName)
                .HasColumnName("bank_name")
                .HasMaxLength(200);

            bank.Property(x => x.TaxNumber)
                .HasColumnName("bank_tax_number")
                .HasMaxLength(50);

            bank.Property(x => x.Swift)
                .HasColumnName("bank_swift")
                .HasMaxLength(50);

            bank.Property(x => x.PurposeTemplate)
                .HasColumnName("bank_purpose_template")
                .HasColumnType("text");
        });

        builder.OwnsOne(x => x.LiqPaySettings, liqpay =>
        {
            liqpay.WithOwner();

            liqpay.Property(x => x.PublicKey)
                .HasColumnName("liqpay_public_key")
                .HasMaxLength(500);

            liqpay.Property(x => x.PrivateKey)
                .HasColumnName("liqpay_private_key")
                .HasMaxLength(500);

            liqpay.Property(x => x.ResultUrl)
                .HasColumnName("liqpay_result_url")
                .HasColumnType("text");

            liqpay.Property(x => x.ServerCallbackUrl)
                .HasColumnName("liqpay_server_callback_url")
                .HasColumnType("text");

            liqpay.Property(x => x.CheckoutEnabled)
                .HasColumnName("liqpay_checkout_enabled");

            liqpay.Property(x => x.PrivatPayEnabled)
                .HasColumnName("liqpay_privatpay_enabled");

            liqpay.Property(x => x.InstallmentsEnabled)
                .HasColumnName("liqpay_installments_enabled");
        });
    }

    private static void ConfigureDates(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();
    }

    private static void ConfigureRelationships(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.HasMany(x => x.Methods)
            .WithOne()
            .HasForeignKey("PaymentProfileId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
            .FindNavigation(nameof(PaymentProfile.Methods))!
            .SetField("_methods");

        builder.Metadata
            .FindNavigation(nameof(PaymentProfile.Methods))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureIndexes(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.HasIndex(x => x.SellerId)
            .HasDatabaseName("ix_payment_profiles_seller_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_payment_profiles_status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_payment_profiles_created_at_utc");
    }
}