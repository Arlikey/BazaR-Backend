using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewsForModeration;

public sealed class GetSellerReviewsForModerationQueryHandler
    : IRequestHandler<GetSellerReviewsForModerationQuery, PagedResult<SellerReviewListItemDto>>
{
    private readonly ISellerReviewReadRepository _reviews;

    public GetSellerReviewsForModerationQueryHandler(ISellerReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<PagedResult<SellerReviewListItemDto>> Handle(
        GetSellerReviewsForModerationQuery request,
        CancellationToken ct)
    {
        return _reviews.GetForModerationAsync(
            request.Status,
            request.SellerId,
            request.Page,
            request.PageSize,
            ct);
    }
}