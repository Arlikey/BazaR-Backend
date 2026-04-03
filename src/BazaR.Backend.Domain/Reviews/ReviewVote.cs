using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews;

public sealed class ReviewVote
{
    public UserId UserId { get; private set; }
    public bool IsHelpful { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private ReviewVote() { }

    private ReviewVote(UserId userId, bool isHelpful, DateTime createdAtUtc)
    {
        UserId = userId;
        IsHelpful = isHelpful;
        CreatedAtUtc = createdAtUtc;
    }

    public static Result<ReviewVote> Create(UserId userId, bool isHelpful)
    {
        if (userId == default)
        {
            return Result<ReviewVote>.Failure(new Error(
                "ReviewVote.User.Required",
                "User is required."));
        }

        return Result<ReviewVote>.Success(
            new ReviewVote(userId, isHelpful, DateTime.UtcNow));
    }

    public void Change(bool isHelpful)
    {
        IsHelpful = isHelpful;
    }
}