using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.EditProductReview;

public sealed class EditProductReviewCommandHandler
    : IRequestHandler<EditProductReviewCommand, Result>
{
    private readonly IProductReviewRepository _reviews;
    private readonly ProductRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public EditProductReviewCommandHandler(
        IProductReviewRepository reviews,
        ProductRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(EditProductReviewCommand request, CancellationToken ct)
    {
        var reviewId = new ProductReviewId(request.ReviewId);
        var authorUserId = new UserId(request.AuthorUserId);

        var review = await _reviews.GetByIdAsync(reviewId, ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        if (review.AuthorUserId != authorUserId)
        {
            return Result.Failure(new Error(
                "ProductReview.Forbidden",
                "You can edit only your own review."));
        }

        if (request.Rating is null &&
            request.Advantages is null &&
            request.Disadvantages is null &&
            request.Body is null)
        {
            return Result.Failure(new Error(
                "ProductReview.Edit.Empty",
                "At least one field must be provided for update."));
        }

        var newRating = request.Rating ?? review.Rating.Value;
        var newAdvantages = request.Advantages ?? review.Advantages;
        var newDisadvantages = request.Disadvantages ?? review.Disadvantages;
        var newBody = request.Body ?? review.Body;

        var result = review.Edit(
            newRating,
            newAdvantages,
            newDisadvantages,
            newBody);

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