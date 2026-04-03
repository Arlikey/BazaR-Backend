using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetPendingSellerReviews;

public sealed class GetPendingSellerReviewsQueryHandler
    : IRequestHandler<GetPendingSellerReviewsQuery, PagedResult<SellerReviewListItemDto>>
{
    private readonly ISellerReviewReadRepository _reviews;

    public GetPendingSellerReviewsQueryHandler(ISellerReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<PagedResult<SellerReviewListItemDto>> Handle(
        GetPendingSellerReviewsQuery request,
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