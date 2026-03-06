using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class ProductReadRepository : IProductReadRepository
{
    private readonly AppDbContext _db;

    public ProductReadRepository(AppDbContext db) => _db = db;

    private static IQueryable<string> MainImageUrlQuery(Product p)
        => (IQueryable<string>)p.Images
            .OrderByDescending(i => i.IsMain)
            .ThenBy(i => i.SortOrder)
            .Select(i => i.Url)
            .Take(1);

    /*public async Task<IReadOnlyList<ProductListItemDto>> ListByCategoryAsync(
        CategoryId categoryId,
        ProductStatus? status,
        CancellationToken ct)
    {
        IQueryable<Product> query = _db.Products.AsNoTracking();

        query = query.Where(p => p.CategoryId == categoryId);

        if (status is not null)
            query = query.Where(p => p.Status == status.Value);

        var rows = await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductListItemDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.CategoryId.Value,
                p.BrandId.HasValue ? p.BrandId.Value.Value : (Guid?)null,
                p.VendorCode != null ? p.VendorCode.Value : null,
                p.Slug != null ? p.Slug.Value : null,
                p.Status.ToString(),
                MainImageUrlQuery(p).FirstOrDefault()
            ))
            .ToListAsync(ct);

        return rows;
    }*/

    public async Task<IReadOnlyList<ProductCardDto>> ListByCategoryAsync(
    CategoryId categoryId,
    ProductStatus? status,
    CancellationToken ct)
    {
        IQueryable<Product> query = _db.Products.AsNoTracking();

        query = query.Where(p => p.CategoryId == categoryId);

        if (status is not null)
            query = query.Where(p => p.Status == status.Value);

        var rows = await query
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Select(p => new ProductCardDto(
                p.Id.Value,
                p.Name,
                p.Slug != null ? p.Slug.Value : null,
                p.Description,
                p.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);

        return rows;
    }

    public async Task<PagedResult<ProductCardDto>> SearchAsync(
    ProductSearchFilter filter,
    Pagination pagination,
    CancellationToken ct)
    {
        var term = (filter.Term ?? "").Trim();
        var page = pagination.SafePage;
        var pageSize = pagination.SafePageSize;

        if (string.IsNullOrWhiteSpace(term))
        {
            return new PagedResult<ProductCardDto>
            {
                Items = Array.Empty<ProductCardDto>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        var words = term.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        IQueryable<Product> query = _db.Products.AsNoTracking()
            .Where(p => p.Status == filter.Status);

        query = query.Where(p => EF.Functions.ILike(p.Name, $"%{term}%"));

        foreach (var word in words)
        {
            var pattern = $"%{word}%";
            query = query.Where(p => EF.Functions.ILike(p.Name, pattern));
        }

        var total = await query.CountAsync(ct);

        var prefixPattern = $"{term}%";

        var items = await query
            .OrderByDescending(p => EF.Functions.ILike(p.Name, term))
            .ThenByDescending(p => EF.Functions.ILike(p.Name, prefixPattern))
            .ThenBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductCardDto(
                p.Id.Value,
                p.Name,
                p.Slug != null ? p.Slug.Value : null,
                p.Description,
                p.Images
                       .OrderByDescending(i => i.IsMain)
                       .ThenBy(i => i.SortOrder)
                       .Select(i => i.Url)
                       .FirstOrDefault()
                ))
            .ToListAsync(ct);

        return new PagedResult<ProductCardDto>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    
    public async Task<IReadOnlyList<ProductCardDto>> ListBySellerAsync(
    SellerId sellerId,
    int limit,
    CancellationToken ct)
    {
        if (limit <= 0) limit = 20;
        if (limit > 100) limit = 100;

        var items = await _db.Products
            .AsNoTracking()
            .Where(p => p.OwnerSellerId == sellerId)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Take(limit)
            .Select(p => new ProductCardDto(
                p.Id.Value,
                p.Name,
                p.Slug != null ? p.Slug.Value : null,
                p.Description,
                p.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault()
                ))
            .ToListAsync(ct);

        return items;
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
                p.Status.ToString(),
                p.Images
                  .OrderByDescending(i => i.IsMain)
                  .ThenBy(i => i.SortOrder)
                  .Select(i => i.Url)
                  .FirstOrDefault(),
                p.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => new ProductImageDto(
                        i.Id,
                        i.Url,
                        i.IsMain,
                        i.SortOrder))
                    .ToList()
            ))
            .SingleOrDefaultAsync(ct);

    public Task<ProductDetailsDto?> GetByIdForOwnerAsync(
        ProductId productId,
        SellerId ownerSellerId,
        CancellationToken ct)
        => _db.Products
            .AsNoTracking()
            .Where(p => p.Id == productId && p.OwnerSellerId == ownerSellerId)
            .Select(p => new ProductDetailsDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.CategoryId.Value,
                p.BrandId.HasValue ? p.BrandId.Value.Value : (Guid?)null,
                p.VendorCode != null ? p.VendorCode.Value : null,
                p.Slug != null ? p.Slug.Value : null,
                p.Status.ToString(),
                MainImageUrlQuery(p).FirstOrDefault(),
                p.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => new ProductImageDto(
                        i.Id,
                        i.Url,
                        i.IsMain,
                        i.SortOrder))
                    .ToList()
            ))
            .FirstOrDefaultAsync(ct);
}