using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.DeleteProductReviewByAdmin;
using BazaR.Backend.Application.Reviews.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Commands.DeleteProductReviewByAdmin;

public sealed class DeleteProductReviewByAdminCommandHandler
    : IRequestHandler<DeleteProductReviewByAdminCommand, Result>
{
    private readonly IProductReviewRepository _reviews;
    private readonly ProductRatingSummaryUpdater _summaryUpdater;
    private readonly IUnitOfWork _uow;

    public DeleteProductReviewByAdminCommandHandler(
        IProductReviewRepository reviews,
        ProductRatingSummaryUpdater summaryUpdater,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _summaryUpdater = summaryUpdater;
        _uow = uow;
    }

    public async Task<Result> Handle(DeleteProductReviewByAdminCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new ProductReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

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