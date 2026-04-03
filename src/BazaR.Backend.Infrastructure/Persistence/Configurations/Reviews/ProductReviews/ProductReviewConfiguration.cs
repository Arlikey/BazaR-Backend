using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BazaR.Backend.Infrastructure.Persistence.Configurations.Reviews.ProductReviews;

public sealed class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
{
    public void Configure(EntityTypeBuilder<ProductReview> builder)
    {
        builder.ToTable("product_reviews");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => new ProductReviewId(value));

        builder.Property(x => x.ProductId)
            .HasColumnName("product_id")
            .HasConversion(
                id => id.Value,
                value => new ProductId(value))
            .IsRequired();

        builder.Property(x => x.AuthorUserId)
            .HasColumnName("author_user_id")
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasColumnName("rating")
            .HasConversion(
                rating => rating.Value,
                value => ReviewRating.Create(value).Value!)
            .IsRequired();

        builder.Property(x => x.Advantages)
            .HasColumnName("advantages")
            .HasMaxLength(1000);

        builder.Property(x => x.Disadvantages)
            .HasColumnName("disadvantages")
            .HasMaxLength(1000);

        builder.Property(x => x.Body)
            .HasColumnName("body")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.Property(x => x.ModeratedAtUtc)
            .HasColumnName("moderated_at_utc");

        builder.Ignore(x => x.Votes);
        builder.Ignore(x => x.HelpfulVotesCount);
        builder.Ignore(x => x.NotHelpfulVotesCount);

        ConfigureVotes(builder);

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("ix_product_reviews_product_id");

        builder.HasIndex(x => x.AuthorUserId)
            .HasDatabaseName("ix_product_reviews_author_user_id");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_product_reviews_status");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_product_reviews_created_at_utc");

        builder.HasIndex(x => new { x.ProductId, x.AuthorUserId })
            .IsUnique()
            .HasDatabaseName("ux_product_reviews_product_author");

        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "ck_product_reviews_rating_range",
                "rating >= 1 AND rating <= 5");
        });
    }

    private static void ConfigureVotes(EntityTypeBuilder<ProductReview> builder)
    {
        builder.OwnsMany<ReviewVote>("_votes", votesBuilder =>
        {
            votesBuilder.ToTable("product_review_votes");

            votesBuilder.WithOwner()
                .HasForeignKey("product_review_id");

            votesBuilder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value))
                .IsRequired();

            votesBuilder.Property(x => x.IsHelpful)
                .HasColumnName("is_helpful")
                .IsRequired();

            votesBuilder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            votesBuilder.HasKey("product_review_id", nameof(ReviewVote.UserId));

            votesBuilder.HasIndex(x => x.UserId)
                .HasDatabaseName("ix_product_review_votes_user_id");
        });
    }
}