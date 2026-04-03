namespace BazaR.Backend.Domain.Reviews.SellerReviews;

public readonly record struct SellerReviewId(Guid Value)
{
    public static SellerReviewId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}