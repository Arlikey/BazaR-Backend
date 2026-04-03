using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Queries.GetProductReviewsForModeration;

public sealed record GetProductReviewsForModerationQuery(
    string? Status,
    Guid? ProductId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<ProductReviewListItemDto>>;