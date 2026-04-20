using BazaR.Backend.Application.Abstractions.Repositories;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.ViewedProducts;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Repositories;

public sealed class ViewedProductRepository : IViewedProductRepository
{
    private readonly AppDbContext _db;

    public ViewedProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task TrackAsync(Guid userId, ProductId productId, CancellationToken ct)
    {
        var existing = await _db.ViewedProducts
            .SingleOrDefaultAsync(
                x => x.UserId == userId && x.ProductId == productId,
                ct);

        if (existing is null)
        {
            await _db.ViewedProducts.AddAsync(
                ViewedProduct.Create(userId, productId),
                ct);

            return;
        }

        existing.MarkViewed();
    }

    public async Task<PagedResult<ProductId>> GetPagedProductIdsByUserAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        if (page <= 0)
            page = 1;

        if (pageSize <= 0)
            pageSize = 10;

        var query = _db.ViewedProducts
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.ViewedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ProductId)
            .ToListAsync(ct);

        return new PagedResult<ProductId>(items, totalCount, page, pageSize);
    }
}