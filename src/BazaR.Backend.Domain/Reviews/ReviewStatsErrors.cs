using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Reviews;

public static class ReviewStatsErrors
{
    public static readonly Error RatingsRequired = new(
        "ReviewStats.Ratings.Required",
        "Ratings are required.");

    public static readonly Error InvalidRatingValue = new(
        "ReviewStats.Rating.Invalid",
        "Rating must be between 1 and 5.");
}