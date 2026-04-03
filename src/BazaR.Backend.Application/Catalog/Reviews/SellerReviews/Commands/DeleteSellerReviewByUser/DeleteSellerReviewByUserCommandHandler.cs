using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.DeleteSellerReviewByUser;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Commands.DeleteSellerReviewByUser;

public sealed class DeleteSellerReviewByUserCommandHandler
    : IRequestHandler<DeleteSellerReviewByUserCommand, Result>
{
    private readonly ISellerReviewRepository _reviews;
    private readonly SellerRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public DeleteSellerReviewByUserCommandHandler(
        ISellerReviewRepository reviews,
        SellerRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteSellerReviewByUserCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new SellerReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        if (review.AuthorUserId != new UserId(request.AuthorUserId))
        {
            return Result.Failure(new Error(
                "SellerReview.Forbidden",
                "You can delete only your own review."));
        }

        var sellerId = review.SellerId;

        _reviews.Remove(review);

        await _uow.SaveChangesAsync(ct);

        var updateSummary = await _summaryUpdater.UpdateAsync(sellerId, ct);
        if (updateSummary.IsFailure)
            return updateSummary;

        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}