using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews.SellerReviews.Events;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.SellerReviews;

public sealed class SellerReview : AggregateRoot<SellerReviewId>
{
    private const int MaxTitleLength = 200;
    private const int MaxBodyLength = 4000;

    private readonly List<ReviewVote> _votes = new();

    public SellerId SellerId { get; private set; }
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
     
    private SellerReview() { }

    private SellerReview(
        SellerReviewId id,
        SellerId sellerId,
        UserId authorUserId,
        ReviewRating rating,
        string title,
        string body)
        : base(id)
    {
        SellerId = sellerId;
        AuthorUserId = authorUserId;
        Rating = rating;
        Title = title;
        Body = body;
        Status = ReviewStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public static Result<SellerReview> Create(
        SellerId sellerId,
        UserId authorUserId,
        int rating,
        string title,
        string body)
    {
        if (sellerId == default)
            return Result<SellerReview>.Failure(ReviewErrors.SellerRequired);

        if (authorUserId == default)
            return Result<SellerReview>.Failure(ReviewErrors.AuthorRequired);

        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return Result<SellerReview>.Failure(ratingResult.Error);

        var normalizedTitle = NormalizeTitle(title);
        if (normalizedTitle is null)
            return Result<SellerReview>.Failure(ReviewErrors.TitleRequired);

        if (normalizedTitle.Length > MaxTitleLength)
            return Result<SellerReview>.Failure(ReviewErrors.TitleTooLong);

        var normalizedBody = NormalizeBody(body);
        if (normalizedBody is null)
            return Result<SellerReview>.Failure(ReviewErrors.BodyRequired);

        if (normalizedBody.Length > MaxBodyLength)
            return Result<SellerReview>.Failure(ReviewErrors.BodyTooLong);

        var review = new SellerReview(
            SellerReviewId.New(),
            sellerId,
            authorUserId,
            ratingResult.Value!,
            normalizedTitle,
            normalizedBody);

        review.AddDomainEvent(new SellerReviewCreatedEvent(
            review.Id,
            review.SellerId,
            review.AuthorUserId));

        return Result<SellerReview>.Success(review);
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

        Status = ReviewStatus.Pending;
        ModeratedAtUtc = null;

        AddDomainEvent(new SellerReviewEditedEvent(
            Id,
            SellerId,
            AuthorUserId));

        return Result.Success();
    }

    public Result DeleteByUser()
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.AlreadyDeleted);

        Status = ReviewStatus.Deleted;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new SellerReviewDeletedByUserEvent(
            Id,
            SellerId,
            AuthorUserId));

        return Result.Success();
    }

    public Result DeleteByAdmin()
    {
        if (Status == ReviewStatus.Deleted)
            return Result.Failure(ReviewErrors.AlreadyDeleted);

        Status = ReviewStatus.Deleted;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new SellerReviewDeletedByAdminEvent(
            Id,
            SellerId,
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

        AddDomainEvent(new SellerReviewApprovedEvent(
            Id,
            SellerId,
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

        AddDomainEvent(new SellerReviewRejectedEvent(
            Id,
            SellerId,
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

        AddDomainEvent(new SellerReviewVotedEvent(
            Id,
            SellerId,
            userId,
            isHelpful));

        return Result.Success();
    }

    private static string? NormalizeTitle(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeBody(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}