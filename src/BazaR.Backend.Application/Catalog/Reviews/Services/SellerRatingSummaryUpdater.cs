using BazaR.Backend.Application.Abstractions.Services;

using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews.SellerRatings;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Reviews.Services;

public sealed class SellerRatingSummaryUpdater
{
    private readonly ISellerReviewRatingReader _ratings;
    private readonly ISellerRatingSummaryRepository _summaries;

    public SellerRatingSummaryUpdater(
        ISellerReviewRatingReader ratings,
        ISellerRatingSummaryRepository summaries)
    {
        _ratings = ratings;
        _summaries = summaries;
    }

    public async Task<Result> UpdateAsync(SellerId sellerId, CancellationToken ct)
    {
        var approvedRatings = await _ratings.GetApprovedRatingsAsync(sellerId, ct);

        var summary = await _summaries.GetByIdAsync(sellerId, ct);
        if (summary is null)
        {
            var created = SellerRatingSummary.Create(sellerId);
            if (created.IsFailure)
                return Result.Failure(created.Error);

            summary = created.Value!;
            await _summaries.AddAsync(summary, ct);
        }

        var recalc = summary.Recalculate(approvedRatings);
        if (recalc.IsFailure)
            return recalc;

        return Result.Success();
    }
}