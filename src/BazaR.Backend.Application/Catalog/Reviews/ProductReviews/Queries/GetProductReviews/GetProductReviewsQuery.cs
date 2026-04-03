using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.Queries.GetProductReviews;

public sealed record GetProductReviewsQuery(
    Guid ProductId,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null) : IRequest<PagedResult<ProductReviewListItemDto>>;