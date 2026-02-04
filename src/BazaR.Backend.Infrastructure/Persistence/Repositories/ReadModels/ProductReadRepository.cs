using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class ProductReadRepository : IProductReadRepository
{
    private readonly AppDbContext _db;

    public ProductReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> ListAsync(CancellationToken ct)
        => await _db.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemDto(
                p.Id.Value,
                p.Name,
                p.CategoryId.Value,
                p.BrandId.HasValue ? p.BrandId.Value.Value : (Guid?)null,
                p.VendorCode != null ? p.VendorCode.Value : null,
                p.Slug != null ? p.Slug.Value : null,
                p.Status.ToString()
            ))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProductListItemDto>> ListByCategoryAsync(CategoryId categoryId, CancellationToken ct)
        => await _db.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemDto(
                p.Id.Value,
                p.Name,
                p.CategoryId.Value,
                p.BrandId.HasValue ? p.BrandId.Value.Value : (Guid?)null,
                p.VendorCode != null ? p.VendorCode.Value : null,
                p.Slug != null ? p.Slug.Value : null,
                p.Status.ToString()
            ))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProductListItemDto>> SearchAsync(string term, int limit, CancellationToken ct)
    {
        term ??= string.Empty;
        var trimmed = term.Trim();

        if (limit <= 0) limit = 10;
        if (limit > 50) limit = 50;

        IQueryable<Product> query = _db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            var pattern = $"%{trimmed}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, pattern) ||
                (p.VendorCode != null && EF.Functions.ILike(p.VendorCode.Value, pattern)) ||
                (p.Slug != null && EF.Functions.ILike(p.Slug.Value, pattern)));
        }

        return await query
            .OrderBy(p => p.Name)
            .Take(limit)
            .Select(p => new ProductListItemDto(
                p.Id.Value,
                p.Name,
                p.CategoryId.Value,
                p.BrandId.HasValue ? p.BrandId.Value.Value : (Guid?)null,
                p.VendorCode != null ? p.VendorCode.Value : null,
                p.Slug != null ? p.Slug.Value : null,
                p.Status.ToString()
            ))
            .ToListAsync(ct);
    }

    public Task<ProductDetailsDto?> GetByIdAsync(ProductId id, CancellationToken ct)
        => _db.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailsDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.CategoryId.Value,
                p.BrandId.HasValue ? p.BrandId.Value.Value : (Guid?)null,
                p.VendorCode != null ? p.VendorCode.Value : null,
                p.Slug != null ? p.Slug.Value : null,
                p.Status.ToString()
            ))
            .SingleOrDefaultAsync(ct);
}
