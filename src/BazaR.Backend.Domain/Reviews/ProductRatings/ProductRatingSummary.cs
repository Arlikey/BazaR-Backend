using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Reviews.ProductRatings;

public sealed class ProductRatingSummary : Entity<ProductId>
{
    public decimal AverageRating { get; private set; }
    public int ReviewsCount { get; private set; }

    public int FiveStarsCount { get; private set; }
    public int FourStarsCount { get; private set; }
    public int ThreeStarsCount { get; private set; }
    public int TwoStarsCount { get; private set; }
    public int OneStarCount { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private ProductRatingSummary(ProductId productId)
        : base(productId)
    {
        AverageRating = 0m;
        ReviewsCount = 0;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private ProductRatingSummary() { }

    public static Result<ProductRatingSummary> Create(ProductId productId)
    {
        if (productId == default)
        {
            return Result<ProductRatingSummary>.Failure(new Error(
                "ProductRatingSummary.Product.Required",
                "Product is required."));
        }

        return Result<ProductRatingSummary>.Success(new ProductRatingSummary(productId));
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