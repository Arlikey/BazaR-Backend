using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewsForModeration;

public sealed class GetProductReviewsForModerationQueryHandler
    : IRequestHandler<GetProductReviewsForModerationQuery, PagedResult<ProductReviewListItemDto>>
{
    private readonly IProductReviewReadRepository _reviews;

    public GetProductReviewsForModerationQueryHandler(IProductReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<PagedResult<ProductReviewListItemDto>> Handle(
        GetProductReviewsForModerationQuery request,
        CancellationToken ct)
    {
        return _reviews.GetForModerationAsync(
            request.Status,
            request.ProductId,
            request.Page,
            request.PageSize,
            ct);
    }
}