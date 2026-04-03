using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;
using BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetPendingProductReviews;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Queries.GetPendingProductReviews;

public sealed class GetPendingProductReviewsQueryHandler
    : IRequestHandler<GetPendingProductReviewsQuery, PagedResult<ProductReviewListItemDto>>
{
    private readonly IProductReviewReadRepository _reviews;

    public GetPendingProductReviewsQueryHandler(IProductReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<PagedResult<ProductReviewListItemDto>> Handle(
        GetPendingProductReviewsQuery request,
        CancellationToken ct)
    {
        return _reviews.GetForModerationAsync(
            "Pending",
            null,
            request.Page,
            request.PageSize,
            ct);
    }
}