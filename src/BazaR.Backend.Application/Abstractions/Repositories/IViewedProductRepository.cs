
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IViewedProductRepository
{
    Task TrackAsync(Guid userId, ProductId productId, CancellationToken ct);

    Task<PagedResult<ProductId>> GetPagedProductIdsByUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct);
}