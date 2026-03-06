using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("offers");

        // ======================
        // Key (OfferId)
        // ======================
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new OfferId(value));

        // ======================
        // Foreign Keys (as value objects)
        // ======================
        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired()
            .HasConversion(id => id.Value, value => new ProductId(value));

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .IsRequired()
            .HasConversion(id => id.Value, value => new SellerId(value));

        // ======================
        // Money (owned) - Price
        // ======================
        builder.OwnsOne(x => x.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("price_amount")
                .HasColumnType("numeric(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3);

            money.WithOwner();
        });
        builder.Navigation(x => x.Price).IsRequired(false);

        // ======================
        // Money (owned) - OldPrice
        // ======================
        builder.OwnsOne(x => x.OldPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("old_price_amount")
                .HasColumnType("numeric(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("old_price_currency")
                .HasMaxLength(3);

            money.WithOwner();
        });
        builder.Navigation(x => x.OldPrice).IsRequired(false);

        // ======================
        // Inventory / Commercial
        // ======================
        builder.Property(x => x.Stock)
            .HasColumnName("stock")
            .IsRequired();

        builder.Property(x => x.SellerSku)
            .HasColumnName("seller_sku")
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(x => x.DeliveryDays)
            .HasColumnName("delivery_days")
            .IsRequired(false);

        builder.Property(x => x.MinOrderQuantity)
            .HasColumnName("min_order_quantity")
            .IsRequired()
            .HasDefaultValue(1);

        // ======================
        // Status
        // ======================
        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        // ======================
        // Audit
        // ======================
        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // ======================
        // Indexes
        // ======================
        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_offers_product_id");

        builder.HasIndex(x => x.SellerId)
            .HasDatabaseName("ix_offers_seller_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_offers_status");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("ix_offers_created_at");

        // Unique pair: (ProductId, SellerId)
        builder.HasIndex(x => new { x.ProductId, x.SellerId })
            .IsUnique()
            .HasDatabaseName("ux_offers_product_id_seller_id");

        // Common query: all offers for product by status
        builder.HasIndex(x => new { x.ProductId, x.Status })
            .HasDatabaseName("ix_offers_product_id_status");

        // Seller SKU: usually unique within seller, not globally
        builder.HasIndex(x => new { x.SellerId, x.SellerSku })
            .HasDatabaseName("ix_offers_seller_id_seller_sku");

        builder.HasIndex(x => new { x.SellerId, x.SellerSku })
            .IsUnique()
            .HasFilter("seller_sku IS NOT NULL")
            .HasDatabaseName("ux_offers_seller_id_seller_sku");

        // ======================
        // Constraints (PostgreSQL style)
        // ======================
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_offers_stock_non_negative", "stock >= 0");

            // delivery_days: allow 0 if you treat it as "same day/instant"
            t.HasCheckConstraint("ck_offers_delivery_days_non_negative", "delivery_days IS NULL OR delivery_days >= 0");

            t.HasCheckConstraint("ck_offers_min_order_quantity_positive", "min_order_quantity >= 1");

            // Amount must be non-negative
            t.HasCheckConstraint("ck_offers_price_amount_non_negative", "price_amount IS NULL OR price_amount >= 0");
            t.HasCheckConstraint("ck_offers_old_price_amount_non_negative", "old_price_amount IS NULL OR old_price_amount >= 0");

            // Currency length if present
            t.HasCheckConstraint("ck_offers_price_currency_len", "price_currency IS NULL OR char_length(price_currency) = 3");
            t.HasCheckConstraint("ck_offers_old_price_currency_len", "old_price_currency IS NULL OR char_length(old_price_currency) = 3");

            
            t.HasCheckConstraint(
                "ck_offers_price_nulls_together",
                "(price_amount IS NULL AND price_currency IS NULL) OR (price_amount IS NOT NULL AND price_currency IS NOT NULL)"
            );

            t.HasCheckConstraint(
                "ck_offers_old_price_nulls_together",
                "(old_price_amount IS NULL AND old_price_currency IS NULL) OR (old_price_amount IS NOT NULL AND old_price_currency IS NOT NULL)"
            );
        });

        // ======================
        // Ignore domain events
        // ======================
        builder.Ignore("DomainEvents");
    }
}