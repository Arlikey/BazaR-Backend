using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviews;

public sealed class GetSellerReviewsQueryHandler
    : IRequestHandler<GetSellerReviewsQuery, PagedResult<SellerReviewListItemDto>>
{
    private readonly ISellerReviewReadRepository _reviews;

    public GetSellerReviewsQueryHandler(ISellerReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<PagedResult<SellerReviewListItemDto>> Handle(
        GetSellerReviewsQuery request,
        CancellationToken ct)
    {
        return _reviews.GetSellerReviewsAsync(
            request.SellerId,
            request.Page,
            request.PageSize,
            request.SortBy,
            ct);
    }
}