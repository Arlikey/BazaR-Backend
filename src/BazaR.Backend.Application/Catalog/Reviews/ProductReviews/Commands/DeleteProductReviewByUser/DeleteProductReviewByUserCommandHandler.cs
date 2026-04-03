using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.DeleteProductReviewByUser;

public sealed class DeleteProductReviewByUserCommandHandler
    : IRequestHandler<DeleteProductReviewByUserCommand, Result>
{
    private readonly IProductReviewRepository _reviews;
    private readonly ProductRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public DeleteProductReviewByUserCommandHandler(
        IProductReviewRepository reviews,
        ProductRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteProductReviewByUserCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new ProductReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        if (review.AuthorUserId != new UserId(request.AuthorUserId))
        {
            return Result.Failure(new Error(
                "ProductReview.Forbidden",
                "You can delete only your own review."));
        }

        var productId = review.ProductId;

        _reviews.Remove(review);

        await _uow.SaveChangesAsync(ct);

        var updateSummary = await _summaryUpdater.UpdateAsync(productId, ct);
        if (updateSummary.IsFailure)
            return updateSummary;

        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}