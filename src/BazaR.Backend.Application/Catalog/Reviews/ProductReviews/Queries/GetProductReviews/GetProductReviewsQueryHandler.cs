using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Queries.GetProductReviews;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviews;

public sealed class GetProductReviewsQueryHandler
    : IRequestHandler<GetProductReviewsQuery, PagedResult<ProductReviewListItemDto>>
{
    private readonly IProductReviewReadRepository _reviews;

    public GetProductReviewsQueryHandler(IProductReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<PagedResult<ProductReviewListItemDto>> Handle(
        GetProductReviewsQuery request,
        CancellationToken ct)
    {
        return _reviews.GetProductReviewsAsync(
            request.ProductId,
            request.Page,
            request.PageSize,
            request.SortBy,
            ct);
    }
}