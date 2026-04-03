using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;

using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewSummary;

public sealed class GetProductReviewSummaryQueryHandler
    : IRequestHandler<GetProductReviewSummaryQuery, ProductReviewSummaryDto?>
{
    private readonly IProductReviewReadRepository _reviews;

    public GetProductReviewSummaryQueryHandler(IProductReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<ProductReviewSummaryDto?> Handle(
        GetProductReviewSummaryQuery request,
        CancellationToken ct)
    {
        return _reviews.GetSummaryAsync(request.ProductId, ct);
    }
}