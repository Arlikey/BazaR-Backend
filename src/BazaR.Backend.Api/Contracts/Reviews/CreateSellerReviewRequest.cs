namespace BazaR.Backend.Api.Contracts.Reviews.SellerReviews;

public sealed class CreateSellerReviewRequest
{
    public Guid SellerId { get; init; }

    public int Rating { get; init; }

    public string? Advantages { get; init; }

    public string? Disadvantages { get; init; }

    public string? Body { get; init; }
}