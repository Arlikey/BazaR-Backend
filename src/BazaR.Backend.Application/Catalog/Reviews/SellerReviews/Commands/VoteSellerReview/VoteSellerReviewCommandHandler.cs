using BazaR.Backend.Application.Abstractions.Persistence;

using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.VoteSellerReview;

public sealed class VoteSellerReviewCommandHandler
    : IRequestHandler<VoteSellerReviewCommand, Result>
{
    private readonly ISellerReviewRepository _reviews;
    private readonly IUnitOfWork _uow;

    public VoteSellerReviewCommandHandler(
        ISellerReviewRepository reviews,
        IUnitOfWork uow)
    {
        _reviews = reviews;
        _uow = uow;
    }

    public async Task<Result> Handle(VoteSellerReviewCommand request, CancellationToken ct)
    {
        var review = await _reviews.GetByIdAsync(new SellerReviewId(request.ReviewId), ct);
        if (review is null)
            return Result.Failure(ReviewErrors.NotFound);

        var result = review.Vote(new UserId(request.UserId), request.IsHelpful);
        if (result.IsFailure)
            return result;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}