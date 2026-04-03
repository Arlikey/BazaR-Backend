namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;

public sealed class SellerReviewListItemDto
{
    public Guid ReviewId { get; init; }
    public Guid SellerId { get; init; }
    public Guid AuthorUserId { get; init; }
    public string? AuthorDisplayName { get; init; }
    public int Rating { get; init; }
    public string? Advantages { get; init; }
    public string? Disadvantages { get; init; }
    public string? Body { get; init; }
    public string Status { get; init; } = default!;
    public int HelpfulVotesCount { get; init; }
    public int NotHelpfulVotesCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public DateTime? ModeratedAtUtc { get; init; }
}