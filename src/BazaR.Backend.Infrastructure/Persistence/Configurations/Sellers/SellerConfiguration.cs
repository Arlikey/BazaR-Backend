using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("sellers");

        // ======================
        // Key (SellerId)
        // ======================
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new SellerId(value));

        // ======================
        // Basic
        // ======================
        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.OwnerUserId)
            .HasColumnName("owner_user_id");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // ======================
        // Storefront (public)
        // ======================
        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000);

        builder.Property(x => x.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(1000);

        // ======================
        // Slug (VO) - owned
        // ======================
        builder.OwnsOne(x => x.Slug, slug =>
        {
            slug.Property(s => s.Value)
                .HasColumnName("slug")
                .HasMaxLength(200)
                .IsRequired();

            slug.HasIndex(s => s.Value)
                .IsUnique()
                .HasDatabaseName("ux_sellers_slug");

            slug.WithOwner();
        });

        // ======================
        // CountryCode (VO) - owned
        // ======================
        builder.OwnsOne(x => x.CountryCode, cc =>
        {
            cc.Property(c => c.Value)
                .HasColumnName("country_code")
                .HasMaxLength(2)
                .IsRequired();

           
            cc.HasIndex(c => c.Value)
                .HasDatabaseName("ix_sellers_country_code");

            cc.WithOwner();
        });

        // ======================
        // Legal / business
        // ======================
        builder.Property(x => x.LegalName)
            .HasColumnName("legal_name")
            .HasMaxLength(300);

        builder.Property(x => x.TaxNumber)
            .HasColumnName("tax_number")
            .HasMaxLength(32);

        // ======================
        // Support contacts (nullable VO) - owned
        // ======================
        builder.OwnsOne(x => x.SupportEmail, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("support_email")
                .HasMaxLength(200);

            email.WithOwner();
        });
        builder.Navigation(x => x.SupportEmail).IsRequired(false);

        builder.OwnsOne(x => x.SupportPhone, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("support_phone")
                .HasMaxLength(50);

            phone.WithOwner();
        });
        builder.Navigation(x => x.SupportPhone).IsRequired(false);

        // ======================
        // Moderation / decision history
        // ======================
        builder.Property(x => x.SubmittedAt)
            .HasColumnName("submitted_at");

        builder.Property(x => x.LastDecisionAt)
            .HasColumnName("last_decision_at");

        builder.Property(x => x.LastDecisionBy)
            .HasColumnName("last_decision_by");

        builder.Property(x => x.LastRejectionReason)
            .HasColumnName("last_rejection_reason")
            .HasMaxLength(2000);

        // ======================
        // Suspension
        // ======================
        builder.Property(x => x.SuspensionReason)
            .HasColumnName("suspension_reason")
            .HasMaxLength(2000);

        builder.Property(x => x.SuspendedAt)
            .HasColumnName("suspended_at");

        builder.Property(x => x.SuspendedBy)
            .HasColumnName("suspended_by");

        // ======================
        // Closing
        // ======================
        builder.Property(x => x.ClosedAt)
            .HasColumnName("closed_at");

        builder.Property(x => x.ClosedBy)
            .HasColumnName("closed_by");

        builder.Property(x => x.CloseReason)
            .HasColumnName("close_reason")
            .HasMaxLength(500);

        // ======================
        // Indexes
        // ======================
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_sellers_status");

        builder.HasIndex(x => x.OwnerUserId)
            .HasDatabaseName("ix_sellers_owner_user_id");

        builder.HasIndex(x => x.SubmittedAt)
            .HasDatabaseName("ix_sellers_submitted_at");

        builder.HasIndex(x => x.TaxNumber)
            .IsUnique()
            .HasFilter("tax_number IS NOT NULL")
            .HasDatabaseName("ux_sellers_tax_number");

      

       
        builder.Ignore("DomainEvents");

      
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_sellers_name_not_empty", "char_length(name) > 0");
            t.HasCheckConstraint("ck_sellers_slug_not_empty", "char_length(slug) > 0");
            t.HasCheckConstraint("ck_sellers_country_code_len", "char_length(country_code) = 2");
        });
    }
}
