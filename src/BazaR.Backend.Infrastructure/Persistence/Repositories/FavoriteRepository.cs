using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Favorites;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class FavoriteRepository : IFavoriteRepository
{
    private readonly AppDbContext _db;

    public FavoriteRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsAsync(
        UserId userId,
        ProductId productId,
        CancellationToken ct)
    {
        return await _db.Favorites
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ProductId == productId, ct);
    }

    public async Task AddAsync(
        Favorite favorite,
        CancellationToken ct)
    {
        await _db.Favorites.AddAsync(favorite, ct);
    }

    public async Task RemoveAsync(
        UserId userId,
        ProductId productId,
        CancellationToken ct)
    {
        var entity = await _db.Favorites
            .SingleOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, ct);

        if (entity is null)
            return;

        _db.Favorites.Remove(entity);
    }

    public async Task<IReadOnlyList<ProductCardDto>> GetProductCardsAsync(
    UserId userId,
    int limit,
    CancellationToken ct)
    {
        if (limit <= 0)
            limit = 20;

        if (limit > 100)
            limit = 100;

        return await (
            from f in _db.Favorites.AsNoTracking()
            join p in _db.Products.AsNoTracking()
                on f.ProductId equals p.Id
            join r in _db.ProductRatingSummaries.AsNoTracking()
                on p.Id equals r.Id into ratings
            from rating in ratings.DefaultIfEmpty()
            where f.UserId == userId
            orderby f.AddedAtUtc descending
            select new ProductCardDto(
                p.Id.Value,
                p.Name,
                p.Slug != null ? p.Slug.Value : null,
                p.Description,
                p.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault(),
                rating != null ? rating.AverageRating : 0m,
                rating != null ? rating.ReviewsCount : 0
            ))
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<HashSet<ProductId>> GetFavoriteProductIdsAsync(
        UserId userId,
        IReadOnlyCollection<ProductId> productIds,
        CancellationToken ct)
    {
        if (productIds is null || productIds.Count == 0)
            return [];

        var ids = await _db.Favorites
            .AsNoTracking()
            .Where(x => x.UserId == userId && productIds.Contains(x.ProductId))
            .Select(x => x.ProductId)
            .ToListAsync(ct);

        return ids.ToHashSet();
    }

    public async Task<bool> IsFavoriteAsync(
        UserId userId,
        ProductId productId,
        CancellationToken ct)
    {
        return await _db.Favorites
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ProductId == productId, ct);
    }
}