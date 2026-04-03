using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Reviews;

public readonly record struct ReviewRating
{
    public int Value { get; }

    private ReviewRating(int value)
    {
        Value = value;
    }

    public static Result<ReviewRating> Create(int value)
    {
        if (value < 1 || value > 5)
        {
            return Result<ReviewRating>.Failure(new Error(
                "Review.Rating.Invalid",
                "Rating must be between 1 and 5."));
        }

        return Result<ReviewRating>.Success(new ReviewRating(value));
    }

    public override string ToString() => Value.ToString();
}