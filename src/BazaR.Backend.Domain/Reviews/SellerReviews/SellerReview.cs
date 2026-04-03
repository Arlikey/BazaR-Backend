using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews.SellerReviews.Events;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.SellerReviews;

public sealed class SellerReview : AggregateRoot<SellerReviewId>
{
    private const int MaxAdvantagesLength = 2000;
    private const int MaxDisadvantagesLength = 2000;
    private const int MaxBodyLength = 4000;

    private readonly List<ReviewVote> _votes = new();

    public SellerId SellerId { get; private set; }
    public UserId AuthorUserId { get; private set; }
    public ReviewRating Rating { get; private set; }

    public string? Advantages { get; private set; }
    public string? Disadvantages { get; private set; }
    public string? Body { get; private set; }

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
        string? advantages,
        string? disadvantages,
        string? body)
        : base(id)
    {
        SellerId = sellerId;
        AuthorUserId = authorUserId;
        Rating = rating;
        Advantages = advantages;
        Disadvantages = disadvantages;
        Body = body;
        Status = ReviewStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public static Result<SellerReview> Create(
        SellerId sellerId,
        UserId authorUserId,
        int rating,
        string? advantages,
        string? disadvantages,
        string? body)
    {
        if (sellerId == default)
            return Result<SellerReview>.Failure(ReviewErrors.SellerRequired);

        if (authorUserId == default)
            return Result<SellerReview>.Failure(ReviewErrors.AuthorRequired);

        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return Result<SellerReview>.Failure(ratingResult.Error);

        var normalizedAdvantages = NormalizeOptional(advantages);
        var normalizedDisadvantages = NormalizeOptional(disadvantages);
        var normalizedBody = NormalizeOptional(body);

        var contentValidation = ValidateContent(
            normalizedAdvantages,
            normalizedDisadvantages,
            normalizedBody);

        if (contentValidation.IsFailure)
            return Result<SellerReview>.Failure(contentValidation.Error);

        var review = new SellerReview(
            SellerReviewId.New(),
            sellerId,
            authorUserId,
            ratingResult.Value!,
            normalizedAdvantages,
            normalizedDisadvantages,
            normalizedBody);

        review.AddDomainEvent(new SellerReviewCreatedEvent(
            review.Id,
            review.SellerId,
            review.AuthorUserId));

        return Result<SellerReview>.Success(review);
    }

    public Result Edit(
        int rating,
        string? advantages,
        string? disadvantages,
        string? body)
    {
        var ratingResult = ReviewRating.Create(rating);
        if (ratingResult.IsFailure)
            return Result.Failure(ratingResult.Error);

        var normalizedAdvantages = NormalizeOptional(advantages);
        var normalizedDisadvantages = NormalizeOptional(disadvantages);
        var normalizedBody = NormalizeOptional(body);

        var contentValidation = ValidateContent(
            normalizedAdvantages,
            normalizedDisadvantages,
            normalizedBody);

        if (contentValidation.IsFailure)
            return contentValidation;

        Rating = ratingResult.Value!;
        Advantages = normalizedAdvantages;
        Disadvantages = normalizedDisadvantages;
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

    public Result Approve()
    {
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

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static Result ValidateContent(
        string? advantages,
        string? disadvantages,
        string? body)
    {
        if (advantages is null && disadvantages is null && body is null)
        {
            return Result.Failure(new Error(
                "Review.Content.Required",
                "At least one of advantages, disadvantages or body must be provided."));
        }

        if (advantages is not null && advantages.Length > MaxAdvantagesLength)
        {
            return Result.Failure(new Error(
                "Review.Advantages.TooLong",
                $"Advantages must not exceed {MaxAdvantagesLength} characters."));
        }

        if (disadvantages is not null && disadvantages.Length > MaxDisadvantagesLength)
        {
            return Result.Failure(new Error(
                "Review.Disadvantages.TooLong",
                $"Disadvantages must not exceed {MaxDisadvantagesLength} characters."));
        }

        if (body is not null && body.Length > MaxBodyLength)
        {
            return Result.Failure(new Error(
                "Review.Body.TooLong",
                $"Body must not exceed {MaxBodyLength} characters."));
        }

        return Result.Success();
    }
}