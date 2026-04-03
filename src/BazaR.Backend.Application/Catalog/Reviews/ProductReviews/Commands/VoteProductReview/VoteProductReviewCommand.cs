using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.VoteProductReview;

public sealed record VoteProductReviewCommand(
    Guid ReviewId,
    Guid UserId,
    bool IsHelpful) : IRequest<Result>;