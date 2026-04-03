namespace BazaR.Backend.Api.Contracts.Reviews.SellerReviews;

public sealed class EditSellerReviewRequest
{
    public int? Rating { get; init; }

    public string? Advantages { get; init; }

    public string? Disadvantages { get; init; }

    public string? Body { get; init; }
}