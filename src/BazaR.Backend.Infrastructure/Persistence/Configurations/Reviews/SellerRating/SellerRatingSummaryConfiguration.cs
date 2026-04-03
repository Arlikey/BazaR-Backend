using BazaR.Backend.Domain.Reviews.SellerRatings;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Reviews.SellerRating;

public sealed class SellerRatingSummaryConfiguration : IEntityTypeConfiguration<SellerRatingSummary>
{
    public void Configure(EntityTypeBuilder<SellerRatingSummary> builder)
    {
        builder.ToTable("seller_rating_summaries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("seller_id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new SellerId(value));

        builder.Property(x => x.AverageRating)
            .HasColumnName("average_rating")
            .HasPrecision(4, 2)
            .IsRequired();

        builder.Property(x => x.ReviewsCount)
            .HasColumnName("reviews_count")
            .IsRequired();

        builder.Property(x => x.FiveStarsCount)
            .HasColumnName("five_stars_count")
            .IsRequired();

        builder.Property(x => x.FourStarsCount)
            .HasColumnName("four_stars_count")
            .IsRequired();

        builder.Property(x => x.ThreeStarsCount)
            .HasColumnName("three_stars_count")
            .IsRequired();

        builder.Property(x => x.TwoStarsCount)
            .HasColumnName("two_stars_count")
            .IsRequired();

        builder.Property(x => x.OneStarCount)
            .HasColumnName("one_star_count")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.HasIndex(x => x.AverageRating)
            .HasDatabaseName("ix_seller_rating_summaries_average_rating");

        builder.HasIndex(x => x.ReviewsCount)
            .HasDatabaseName("ix_seller_rating_summaries_reviews_count");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "ck_seller_rating_summaries_average_rating_range",
                "average_rating >= 0 AND average_rating <= 5");

            t.HasCheckConstraint(
                "ck_seller_rating_summaries_reviews_count_non_negative",
                "reviews_count >= 0");

            t.HasCheckConstraint(
                "ck_seller_rating_summaries_five_stars_count_non_negative",
                "five_stars_count >= 0");

            t.HasCheckConstraint(
                "ck_seller_rating_summaries_four_stars_count_non_negative",
                "four_stars_count >= 0");

            t.HasCheckConstraint(
                "ck_seller_rating_summaries_three_stars_count_non_negative",
                "three_stars_count >= 0");

            t.HasCheckConstraint(
                "ck_seller_rating_summaries_two_stars_count_non_negative",
                "two_stars_count >= 0");

            t.HasCheckConstraint(
                "ck_seller_rating_summaries_one_star_count_non_negative",
                "one_star_count >= 0");
        });
    }
}