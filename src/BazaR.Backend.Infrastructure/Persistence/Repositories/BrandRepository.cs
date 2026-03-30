using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Repositories;

public sealed class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _db;

    public BrandRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Brand?> GetByIdAsync(BrandId id, CancellationToken ct = default)
    {
        return await _db.Brands
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Brand?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        return await _db.Brands
            .FirstOrDefaultAsync(x => x.Slug == normalizedSlug, ct);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var normalizedName = name.Trim();

        return await _db.Brands
            .AnyAsync(x => x.Name == normalizedName, ct);
    }

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return false;

        var normalizedSlug = slug.Trim().ToLowerInvariant();

        return await _db.Brands
            .AnyAsync(x => x.Slug == normalizedSlug, ct);
    }

    public async Task AddAsync(Brand brand, CancellationToken ct = default)
    {
        await _db.Brands.AddAsync(brand, ct);
    }
}