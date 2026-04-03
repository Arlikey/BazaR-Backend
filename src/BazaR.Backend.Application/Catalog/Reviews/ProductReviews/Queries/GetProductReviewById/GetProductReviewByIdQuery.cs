using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;

using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewById;

public sealed record GetProductReviewByIdQuery(Guid ReviewId)
    : IRequest<ProductReviewDetailsDto?>;