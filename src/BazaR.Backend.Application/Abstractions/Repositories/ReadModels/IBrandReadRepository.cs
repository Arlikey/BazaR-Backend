
using BazaR.Backend.Application.Sellers.DTOs;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IBrandReadRepository
{
    Task<BrandDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<BrandLookupDto?> GetLookupByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<BrandLookupDto>> SearchActiveAsync(
        string? search,
        int limit,
        CancellationToken ct = default);

    Task<PagedResult<BrandListItemDto>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken ct = default);
}