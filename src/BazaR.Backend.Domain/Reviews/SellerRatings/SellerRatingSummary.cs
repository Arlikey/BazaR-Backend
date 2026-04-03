using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.Reviews.SellerRatings;

public sealed class SellerRatingSummary : Entity<SellerId>
{
    public decimal AverageRating { get; private set; }
    public int ReviewsCount { get; private set; }

    public int FiveStarsCount { get; private set; }
    public int FourStarsCount { get; private set; }
    public int ThreeStarsCount { get; private set; }
    public int TwoStarsCount { get; private set; }
    public int OneStarCount { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private SellerRatingSummary(SellerId sellerId)
        : base(sellerId)
    {
        AverageRating = 0m;
        ReviewsCount = 0;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private SellerRatingSummary() { }

    public static Result<SellerRatingSummary> Create(SellerId sellerId)
    {
        if (sellerId == default)
        {
            return Result<SellerRatingSummary>.Failure(new Error(
                "SellerRatingSummary.Seller.Required",
                "Seller is required."));
        }

        return Result<SellerRatingSummary>.Success(new SellerRatingSummary(sellerId));
    }

    public Result Recalculate(IReadOnlyCollection<int> approvedRatings)
    {
        if (approvedRatings is null)
            return Result.Failure(ReviewStatsErrors.RatingsRequired);

        if (approvedRatings.Any(x => x < 1 || x > 5))
            return Result.Failure(ReviewStatsErrors.InvalidRatingValue);

        ReviewsCount = approvedRatings.Count;

        FiveStarsCount = approvedRatings.Count(x => x == 5);
        FourStarsCount = approvedRatings.Count(x => x == 4);
        ThreeStarsCount = approvedRatings.Count(x => x == 3);
        TwoStarsCount = approvedRatings.Count(x => x == 2);
        OneStarCount = approvedRatings.Count(x => x == 1);

        AverageRating = ReviewsCount == 0
            ? 0m
            : Math.Round((decimal)approvedRatings.Average(), 2, MidpointRounding.AwayFromZero);

        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}