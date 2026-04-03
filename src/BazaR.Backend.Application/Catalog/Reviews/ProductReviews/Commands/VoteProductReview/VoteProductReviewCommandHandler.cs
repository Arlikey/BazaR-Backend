using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.VoteProductReview;

public sealed class VoteProductReviewCommandHandler
    : IRequestHandler<VoteProductReviewCommand, Result>
{
    private readonly IProductReviewRepository _reviews;
    private readonly IUnitOfWork _uow;

    public VoteProductReviewCommandHandler(
        IProductReviewRepository reviews,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _uow = uow;
    }

    public async Task<Result> Handle(VoteProductReviewCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new ProductReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        var result = review.Vote(new UserId(request.UserId), request.IsHelpful);
        if (result.IsFailure)
            return result;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}