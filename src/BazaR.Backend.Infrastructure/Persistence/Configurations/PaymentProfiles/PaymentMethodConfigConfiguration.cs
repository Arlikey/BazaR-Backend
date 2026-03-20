using BazaR.Backend.Domain.PaymentProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.PaymentProfiles;

public sealed class PaymentMethodConfigConfiguration : IEntityTypeConfiguration<PaymentMethodConfig>
{
    public void Configure(EntityTypeBuilder<PaymentMethodConfig> builder)
    {
        builder.ToTable("payment_profile_methods");

        ConfigureKeys(builder);
        ConfigureProperties(builder);
        ConfigureIndexes(builder);
    }

    private static void ConfigureKeys(EntityTypeBuilder<PaymentMethodConfig> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new PaymentMethodConfigId(value));

        builder.Property<PaymentProfileId>("PaymentProfileId")
            .HasColumnName("payment_profile_id")
            .HasConversion(
                id => id.Value,
                value => new PaymentProfileId(value))
            .IsRequired();
    }

    private static void ConfigureProperties(EntityTypeBuilder<PaymentMethodConfig> builder)
    {
        builder.Property(x => x.MethodType)
            .HasColumnName("method_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.IsEnabled)
            .HasColumnName("is_enabled")
            .IsRequired();

        builder.Property(x => x.RequiresOnlineAuthorization)
            .HasColumnName("requires_online_authorization")
            .IsRequired();

        builder.Property(x => x.RequiresBankAccount)
            .HasColumnName("requires_bank_account")
            .IsRequired();

        builder.Property(x => x.RequiresLiqPay)
            .HasColumnName("requires_liqpay")
            .IsRequired();

        builder.Property(x => x.MinAmount)
            .HasColumnName("min_amount")
            .HasPrecision(18, 2);

        builder.Property(x => x.MaxAmount)
            .HasColumnName("max_amount")
            .HasPrecision(18, 2);

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasColumnType("text");
    }

    private static void ConfigureIndexes(EntityTypeBuilder<PaymentMethodConfig> builder)
    {
        builder.HasIndex("PaymentProfileId")
            .HasDatabaseName("ix_payment_profile_methods_profile_id");

        builder.HasIndex(x => x.MethodType)
            .HasDatabaseName("ix_payment_profile_methods_method_type");

        builder.HasIndex("PaymentProfileId", nameof(PaymentMethodConfig.MethodType))
            .IsUnique()
            .HasDatabaseName("ux_payment_profile_methods_profile_id_method_type");
    }
}