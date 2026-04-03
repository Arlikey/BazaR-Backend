using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.DeleteProductReviewByUser;

public sealed record DeleteProductReviewByUserCommand(
    Guid ReviewId,
    Guid AuthorUserId) : IRequest<Result>;