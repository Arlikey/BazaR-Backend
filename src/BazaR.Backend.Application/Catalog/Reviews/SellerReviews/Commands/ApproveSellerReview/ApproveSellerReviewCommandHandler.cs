using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.ApproveSellerReview;

public sealed class ApproveSellerReviewCommandHandler
    : IRequestHandler<ApproveSellerReviewCommand, Result>
{
    private readonly ISellerReviewRepository _reviews;
    private readonly SellerRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public ApproveSellerReviewCommandHandler(
        ISellerReviewRepository reviews,
        SellerRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(ApproveSellerReviewCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new SellerReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        var result = review.Approve();
        if (result.IsFailure)
            return result;
        await _uow.SaveChangesAsync(ct);

        var updateSummary = await _summaryUpdater.UpdateAsync(review.SellerId, ct);
        if (updateSummary.IsFailure)
            return updateSummary;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}