using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews.ProductReviews.Events;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.ProductReviews;

public sealed class ProductReview : AggregateRoot<ProductReviewId>
{
    private const int MaxTitleLength = 200;
    private const int MaxBodyLength = 4000;

    private readonly List<ReviewVote> _votes = new();

    public ProductId ProductId { get; private set; }
    public UserId AuthorUserId { get; private set; }
    public ReviewRating Rating { get; private set; }
    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public ReviewStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? ModeratedAtUtc { get; private set; }

    public IReadOnlyCollection<ReviewVote> Votes => _votes.AsReadOnly();

    public int HelpfulVotesCount => _votes.Count(x => x.IsHelpful);
    public int NotHelpfulVotesCount => _votes.Count(x => !x.IsHelpful);

    private ProductReview() { }

    private ProductReview(
        ProductReviewId id,
        ProductId productId,
        UserId authorUserId,
        ReviewRating rating,
        string title,
        string body)
        : base(id)
    {
        ProductId = productId;
        AuthorUserId = authorUserId;
        Rating = rating;
        Title = title;
        Body = body;
        Status = ReviewStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public static Result<ProductReview> Create(
        ProductId productId,
        UserId authorUserId,
        int rating,
        string title,
        string body)
    {
        if (productId == default)
            return Result<ProductReview>.Failure(ReviewErrors.ProductRequired);

        if (authorUserId == default)
            return Result<ProductReview>.Failure(ReviewErrors.AuthorRequired);

        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return Result<ProductReview>.Failure(ratingResult.Error);

        var normalizedTitle = NormalizeTitle(title);
        if (normalizedTitle is null)
            return Result<ProductReview>.Failure(ReviewErrors.TitleRequired);

        if (normalizedTitle.Length > MaxTitleLength)
            return Result<ProductReview>.Failure(ReviewErrors.TitleTooLong);

        var normalizedBody = NormalizeBody(body);
        if (normalizedBody is null)
            return Result<ProductReview>.Failure(ReviewErrors.BodyRequired);

        if (normalizedBody.Length > MaxBodyLength)
            return Result<ProductReview>.Failure(ReviewErrors.BodyTooLong);

        var review = new ProductReview(
            ProductReviewId.New(),
            productId,
            authorUserId,
            ratingResult.Value!,
            normalizedTitle,
            normalizedBody);

        review.AddDomainEvent(new ProductReviewCreatedEvent(
            review.Id,
            review.ProductId,
            review.AuthorUserId));

        return Result<ProductReview>.Success(review);
    }

    public Result Edit(int rating, string title, string body)
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.CannotEditDeleted);

        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return Result.Failure(ratingResult.Error);

        var normalizedTitle = NormalizeTitle(title);
        if (normalizedTitle is null)
            return Result.Failure(ReviewErrors.TitleRequired);

        if (normalizedTitle.Length > MaxTitleLength)
            return Result.Failure(ReviewErrors.TitleTooLong);

        var normalizedBody = NormalizeBody(body);
        if (normalizedBody is null)
            return Result.Failure(ReviewErrors.BodyRequired);

        if (normalizedBody.Length > MaxBodyLength)
            return Result.Failure(ReviewErrors.BodyTooLong);

        Rating = ratingResult.Value!;
        Title = normalizedTitle;
        Body = normalizedBody;
        UpdatedAtUtc = DateTime.UtcNow;

        // после редактирования обычно отзыв снова уходит на модерацию
        Status = ReviewStatus.Pending;
        ModeratedAtUtc = null;

        AddDomainEvent(new ProductReviewEditedEvent(
            Id,
            ProductId,
            AuthorUserId));

        return Result.Success();
    }

    public Result DeleteByUser()
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.AlreadyDeleted);

        Status = ReviewStatus.Deleted;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new ProductReviewDeletedByUserEvent(
            Id,
            ProductId,
            AuthorUserId));

        return Result.Success();
    }

    public Result DeleteByAdmin()
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.AlreadyDeleted);

        Status = ReviewStatus.Deleted;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new ProductReviewDeletedByAdminEvent(
            Id,
            ProductId,
            AuthorUserId));

        return Result.Success();
    }

    public Result Approve()
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.CannotModerateDeleted);

        Status = ReviewStatus.Approved;
        ModeratedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = ModeratedAtUtc.Value;

        AddDomainEvent(new ProductReviewApprovedEvent(
            Id,
            ProductId,
            AuthorUserId));

        return Result.Success();
    }

    public Result Reject()
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.CannotModerateDeleted);

        Status = ReviewStatus.Rejected;
        ModeratedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = ModeratedAtUtc.Value;

        AddDomainEvent(new ProductReviewRejectedEvent(
            Id,
            ProductId,
            AuthorUserId));

        return Result.Success();
    }

    public Result Vote(UserId userId, bool isHelpful)
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.CannotVoteDeleted);

        if (userId == default)
            return Result.Failure(ReviewErrors.AuthorRequired);

        if (userId == AuthorUserId)
            return Result.Failure(ReviewErrors.CannotVoteOwnReview);

        var existing = _votes.SingleOrDefault(x => x.UserId == userId);
        if (existing is null)
        {
            var voteResult = ReviewVote.Create(userId, isHelpful);
            if (voteResult.IsFailure)
                return Result.Failure(voteResult.Error);

            _votes.Add(voteResult.Value!);
        }
        else
        {
            existing.Change(isHelpful);
        }

        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new ProductReviewVotedEvent(
            Id,
            ProductId,
            userId,
            isHelpful));

        return Result.Success();
    }

    private static string? NormalizeTitle(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeBody(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}