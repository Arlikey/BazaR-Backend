using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;

using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewById;

public sealed class GetProductReviewByIdQueryHandler
    : IRequestHandler<GetProductReviewByIdQuery, ProductReviewDetailsDto?>
{
    private readonly IProductReviewReadRepository _reviews;

    public GetProductReviewByIdQueryHandler(IProductReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<ProductReviewDetailsDto?> Handle(
        GetProductReviewByIdQuery request,
        CancellationToken ct)
    {
        return _reviews.GetByIdAsync(request.ReviewId, ct);
    }
}