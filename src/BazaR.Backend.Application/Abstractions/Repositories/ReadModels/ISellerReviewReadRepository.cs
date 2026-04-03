using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;

namespace BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;

public interface ISellerReviewReadRepository
{
    Task<PagedResult<SellerReviewListItemDto>> GetSellerReviewsAsync(
        Guid sellerId,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default);

    Task<SellerReviewDetailsDto?> GetByIdAsync(
        Guid reviewId,
        CancellationToken ct = default);

    Task<SellerReviewSummaryDto?> GetSummaryAsync(
        Guid sellerId,
        CancellationToken ct = default);

    Task<PagedResult<SellerReviewListItemDto>> GetForModerationAsync(
        string? status,
        Guid? sellerId,
        int page,
        int pageSize,
        CancellationToken ct = default);
}