using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;

using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewSummary;

public sealed class GetSellerReviewSummaryQueryHandler
    : IRequestHandler<GetSellerReviewSummaryQuery, SellerReviewSummaryDto?>
{
    private readonly ISellerReviewReadRepository _reviews;

    public GetSellerReviewSummaryQueryHandler(ISellerReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<SellerReviewSummaryDto?> Handle(
        GetSellerReviewSummaryQuery request,
        CancellationToken ct)
    {
        return _reviews.GetSummaryAsync(request.SellerId, ct);
    }
}