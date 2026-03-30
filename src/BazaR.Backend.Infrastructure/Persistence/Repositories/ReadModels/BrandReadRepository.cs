using BazaR.Backend.Application.Abstractions.ReadModels;

using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Repositories;

public sealed class BrandReadRepository : IBrandReadRepository
{
    private readonly AppDbContext _db;

    public BrandReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BrandDetailsDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Brands
            .AsNoTracking()
            .Where(x => x.Id == new BrandId(id))
            .Select(x => new BrandDetailsDto
            {
                Id = x.Id.Value,
                Name = x.Name,
                Slug = x.Slug,
                LogoUrl = x.LogoUrl,
                Description = x.Description,
                Status = x.Status.ToString(),
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedAtUtc = x.UpdatedAtUtc
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<BrandLookupDto?> GetLookupByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Brands
            .AsNoTracking()
            .Where(x => x.Id == new BrandId(id))
            .Select(x => new BrandLookupDto
            {
                Id = x.Id.Value,
                Name = x.Name,
                Slug = x.Slug,
                LogoUrl = x.LogoUrl
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<BrandLookupDto>> SearchActiveAsync(
        string? search,
        int limit,
        CancellationToken ct = default)
    {
        if (limit <= 0)
            limit = 20;

        IQueryable<Brand> query = _db.Brands
            .AsNoTracking()
            .Where(x => x.Status == BrandStatus.Active);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.Name, pattern) ||
                EF.Functions.ILike(x.Slug, pattern));
        }

        return await query
            .OrderBy(x => x.Name)
            .Take(limit)
            .Select(x => new BrandLookupDto
            {
                Id = x.Id.Value,
                Name = x.Name,
                Slug = x.Slug,
                LogoUrl = x.LogoUrl
            })
            .ToListAsync(ct);
    }

    public async Task<PagedResult<BrandListItemDto>> GetPagedAsync(
        string? search,
        string? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<Brand> query = _db.Brands.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";

            query = query.Where(x =>
                EF.Functions.ILike(x.Name, pattern) ||
                EF.Functions.ILike(x.Slug, pattern));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<BrandStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new BrandListItemDto
            {
                Id = x.Id.Value,
                Name = x.Name,
                Slug = x.Slug,
                LogoUrl = x.LogoUrl,
                Status = x.Status.ToString()
            })
            .ToListAsync(ct);

        return new PagedResult<BrandListItemDto>(items, totalCount, page, pageSize);
    }
}