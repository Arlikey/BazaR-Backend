namespace BazaR.Backend.Api.Contracts.Reviews.SellerReviews;

public sealed class CreateSellerReviewRequest
{
    public Guid SellerId { get; init; }
    public int Rating { get; init; }
    public string Title { get; init; } = default!;
    public string Body { get; init; } = default!;
}