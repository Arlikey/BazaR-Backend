using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.EditSellerReview;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Commands.EditSellerReview;

public sealed class EditSellerReviewCommandHandler
    : IRequestHandler<EditSellerReviewCommand, Result>
{
    private readonly ISellerReviewRepository _reviews;
    private readonly SellerRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public EditSellerReviewCommandHandler(
        ISellerReviewRepository reviews,
        SellerRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(EditSellerReviewCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new SellerReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        if (review.AuthorUserId != new UserId(request.AuthorUserId))
        {
            return Result.Failure(new Error(
                "SellerReview.Forbidden",
                "You can edit only your own review."));
        }

        if (request.Rating is null && request.Title is null && request.Body is null)
        {
            return Result.Failure(new Error(
                "SellerReview.Edit.Empty",
                "At least one field must be provided for update."));
        }

        var newRating = request.Rating ?? review.Rating.Value;
        var newTitle = request.Title ?? review.Title;
        var newBody = request.Body ?? review.Body;

        var result = review.Edit(newRating, newTitle, newBody);
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