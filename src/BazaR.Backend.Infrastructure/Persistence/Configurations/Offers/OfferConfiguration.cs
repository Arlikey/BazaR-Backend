using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations;

public sealed class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("offers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => new OfferId(value));

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .IsRequired()
            .HasConversion(id => id.Value, value => new ProductId(value));

        builder.Property(x => x.SellerId)
            .HasColumnName("seller_id")
            .IsRequired()
            .HasConversion(id => id.Value, value => new SellerId(value));

        builder.Property(x => x.Stock)
            .HasColumnName("stock")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

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

        builder.HasIndex(x => x.ProductId).HasDatabaseName("ix_offers_product_id");
        builder.HasIndex(x => x.SellerId).HasDatabaseName("ix_offers_seller_id");

        builder.HasIndex(x => new { x.ProductId, x.SellerId })
            .IsUnique()
            .HasDatabaseName("ux_offers_product_id_seller_id");

        builder.HasIndex(x => new { x.ProductId, x.Status })
            .HasDatabaseName("ix_offers_product_id_status");

        builder.Ignore("DomainEvents");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("ck_offers_stock_non_negative", "stock >= 0");
            t.HasCheckConstraint("ck_offers_price_amount_non_negative", "price_amount IS NULL OR price_amount >= 0");
            t.HasCheckConstraint("ck_offers_price_currency_len", "price_currency IS NULL OR char_length(price_currency) = 3");
        });
    }

}
