using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.CreateProductReview;

public sealed record CreateProductReviewCommand(
    Guid ProductId,
    Guid AuthorUserId,
    int Rating,
    string Title,
    string Body) : IRequest<Result<Guid>>;