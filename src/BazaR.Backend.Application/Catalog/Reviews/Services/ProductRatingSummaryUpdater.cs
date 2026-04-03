using BazaR.Backend.Application.Abstractions.Services;

using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews.ProductRatings;

namespace BazaR.Backend.Application.Reviews.Services;

public sealed class ProductRatingSummaryUpdater {
    private readonly IProductReviewRatingReader _ratings;
    private readonly IProductRatingSummaryRepository _summaries;

    public ProductRatingSummaryUpdater(
        IProductReviewRatingReader ratings,
        IProductRatingSummaryRepository summaries)
    {
        _ratings = ratings;
        _summaries = summaries;
    }

    public async Task<Result> UpdateAsync(ProductId productId, CancellationToken ct)
    {
        var approvedRatings = await _ratings.GetApprovedRatingsAsync(productId, ct);

        var summary = await _summaries.GetByIdAsync(productId, ct);
        if (summary is null)
        {
            var created = ProductRatingSummary.Create(productId);
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