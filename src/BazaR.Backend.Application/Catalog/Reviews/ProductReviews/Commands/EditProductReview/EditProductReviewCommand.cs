using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Commands.EditProductReview;

public sealed record EditProductReviewCommand(
    Guid ReviewId,
    Guid AuthorUserId,
    int? Rating,
    string? Title,
    string? Body) : IRequest<Result>;