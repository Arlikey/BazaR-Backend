using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.Commands.VoteSellerReview;

public sealed record VoteSellerReviewCommand(
    Guid ReviewId,
    Guid UserId,
    bool IsHelpful) : IRequest<Result>;