namespace BazaR.Backend.Domain.Reviews.ProductReviews;

public readonly record struct ProductReviewId(Guid Value)
{
    public static ProductReviewId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}