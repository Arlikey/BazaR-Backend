using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.ApproveProductReview;

public sealed class ApproveProductReviewCommandHandler
    : IRequestHandler<ApproveProductReviewCommand, Result>
{
    private readonly IProductReviewRepository _reviews;
    private readonly ProductRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public ApproveProductReviewCommandHandler(
        IProductReviewRepository reviews,
        ProductRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(ApproveProductReviewCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new ProductReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        var result = review.Approve();
        if (result.IsFailure)
            return result;
        await _uow.SaveChangesAsync(ct);

        var updateSummary = await _summaryUpdater.UpdateAsync(review.ProductId, ct);
        if (updateSummary.IsFailure)
            return updateSummary;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}