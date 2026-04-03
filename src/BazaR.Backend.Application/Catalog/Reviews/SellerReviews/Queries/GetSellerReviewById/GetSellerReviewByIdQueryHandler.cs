using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;

using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetSellerReviewById;

public sealed class GetSellerReviewByIdQueryHandler
    : IRequestHandler<GetSellerReviewByIdQuery, SellerReviewDetailsDto?>
{
    private readonly ISellerReviewReadRepository _reviews;

    public GetSellerReviewByIdQueryHandler(ISellerReviewReadRepository reviews)
    {
        _reviews = reviews;
    }

    public Task<SellerReviewDetailsDto?> Handle(
        GetSellerReviewByIdQuery request,
        CancellationToken ct)
    {
        return _reviews.GetByIdAsync(request.ReviewId, ct);
    }
}