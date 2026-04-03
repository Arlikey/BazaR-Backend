using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;

namespace BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;

public interface IProductReviewReadRepository
{
    Task<PagedResult<ProductReviewListItemDto>> GetProductReviewsAsync(
        Guid productId,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default);

    Task<ProductReviewDetailsDto?> GetByIdAsync(
        Guid reviewId,
        CancellationToken ct = default);

    Task<ProductReviewSummaryDto?> GetSummaryAsync(
        Guid productId,
        CancellationToken ct = default);

    Task<PagedResult<ProductReviewListItemDto>> GetForModerationAsync(
        string? status,
        Guid? productId,
        int page,
        int pageSize,
        CancellationToken ct = default);
}