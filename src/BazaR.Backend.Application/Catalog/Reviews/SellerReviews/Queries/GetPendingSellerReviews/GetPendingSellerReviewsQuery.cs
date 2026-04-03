using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Queries.GetPendingSellerReviews;

public sealed record GetPendingSellerReviewsQuery(
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<SellerReviewListItemDto>>;