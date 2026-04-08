using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Browsing.Abstractions;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BazaR.Backend.Infrastructure.Repositories;

public sealed class CatalogBrowseReadRepository : ICatalogBrowseReadRepository
{
    private readonly AppDbContext _db;

    public CatalogBrowseReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ProductCardDto>> BrowseCategoryProductsAsync(
        Guid categoryId,
        IReadOnlyCollection<CatalogSelectedFilterDto> filters,
        CatalogSystemFiltersDto? systemFilters,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default)
    {
        Console.WriteLine($"\n=== BrowseCategoryProductsAsync START ===");
        Console.WriteLine($"CategoryId: {categoryId}");
        Console.WriteLine($"Attribute filters count: {filters?.Count ?? 0}");
        Console.WriteLine($"System filters: BrandIds={systemFilters?.BrandIds.Count ?? 0}, SellerGroups={systemFilters?.SellerGroups.Count ?? 0}, PriceMin={systemFilters?.PriceMin}, PriceMax={systemFilters?.PriceMax}");
        Console.WriteLine($"Page: {page}, PageSize: {pageSize}, SortBy: {sortBy ?? "null"}");

        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<Product> query = _db.Products
            .AsNoTracking()
            .Where(x => x.CategoryId == new CategoryId(categoryId))
            .Where(x => x.Status == ProductStatus.Published);

        Console.WriteLine("Base query built (category + published).");

        var filteredProductIds = await GetFilteredProductIdsAsync(filters, systemFilters, ct);
        Console.WriteLine($"FilteredProductIds count: {filteredProductIds.Count}");

        var hasAttributeFilters = filters is not null && filters.Count > 0;
        var hasSystemFilters =
            systemFilters is not null &&
            (
                systemFilters.BrandIds.Count > 0 ||
                systemFilters.SellerGroups.Count > 0 ||
                systemFilters.PriceMin.HasValue ||
                systemFilters.PriceMax.HasValue
            );

        if (hasAttributeFilters || hasSystemFilters)
        {
            if (filteredProductIds.Count == 0)
            {
                Console.WriteLine("No products after filtering, returning empty result.");
                return new PagedResult<ProductCardDto>(
                    Array.Empty<ProductCardDto>(),
                    0,
                    page,
                    pageSize);
            }

            var productIds = filteredProductIds
                .Select(g => new ProductId(g))
                .ToList();

            query = query.Where(x => productIds.Contains(x.Id));
            Console.WriteLine($"Applied filter on query, remaining products: {filteredProductIds.Count}");
        }

        query = ApplySorting(query, sortBy);

        var totalCount = await query.CountAsync(ct);
        Console.WriteLine($"Total count after sorting: {totalCount}");

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ProductCardDto(
                x.Id.Value,
                x.Name,
                x.Slug != null ? x.Slug.Value : null,
                x.Description,
                x.Images
                    .OrderByDescending(i => i.IsMain)
                    .ThenBy(i => i.SortOrder)
                    .Select(i => i.Url)
                    .FirstOrDefault()
            ))
            .ToListAsync(ct);

        Console.WriteLine($"Returning {items.Count} items out of {totalCount}");
        Console.WriteLine($"=== BrowseCategoryProductsAsync END ===\n");

        return new PagedResult<ProductCardDto>(items, totalCount, page, pageSize);
    }

    private async Task<List<Guid>> GetFilteredProductIdsAsync(
        IReadOnlyCollection<CatalogSelectedFilterDto>? filters,
        CatalogSystemFiltersDto? systemFilters,
        CancellationToken ct)
    {
        HashSet<Guid>? resultIds = null;

        if (filters is not null)
        {
            foreach (var filter in filters)
            {
                var ids = filter switch
                {
                    CatalogSelectFilterDto x => await GetSelectFilterProductIdsAsync(x, ct),
                    CatalogMultiSelectFilterDto x => await GetMultiSelectFilterProductIdsAsync(x, ct),
                    CatalogBooleanFilterDto x => await GetBooleanFilterProductIdsAsync(x, ct),
                    CatalogNumberRangeFilterDto x => await GetNumberRangeFilterProductIdsAsync(x, ct),
                    CatalogTextFilterDto x => await GetTextFilterProductIdsAsync(x, ct),
                    _ => new List<Guid>()
                };

                resultIds = ApplyIntersection(resultIds, ids);

                if (resultIds.Count == 0)
                    return [];
            }
        }

        if (systemFilters is not null)
        {
            if (systemFilters.BrandIds.Count > 0)
            {
                var ids = await GetBrandFilterProductIdsAsync(systemFilters.BrandIds, ct);
                resultIds = ApplyIntersection(resultIds, ids);

                if (resultIds.Count == 0)
                    return [];
            }

            if (systemFilters.SellerGroups.Count > 0)
            {
                var ids = await GetSellerGroupFilterProductIdsAsync(systemFilters.SellerGroups, ct);
                resultIds = ApplyIntersection(resultIds, ids);

                if (resultIds.Count == 0)
                    return [];
            }

            if (systemFilters.PriceMin.HasValue || systemFilters.PriceMax.HasValue)
            {
                var ids = await GetPriceFilterProductIdsAsync(systemFilters.PriceMin, systemFilters.PriceMax, ct);
                resultIds = ApplyIntersection(resultIds, ids);

                if (resultIds.Count == 0)
                    return [];
            }
        }

        return resultIds?.ToList() ?? [];
    }

    private static HashSet<Guid> ApplyIntersection(
        HashSet<Guid>? resultIds,
        List<Guid> ids)
    {
        var currentSet = ids.ToHashSet();

        if (resultIds is null)
            return currentSet;

        resultIds.IntersectWith(currentSet);
        return resultIds;
    }

    private async Task<List<Guid>> GetSelectFilterProductIdsAsync(
        CatalogSelectFilterDto filter,
        CancellationToken ct)
    {
        Console.WriteLine($"GetSelectFilterProductIdsAsync: AttributeId={filter.AttributeId}, OptionIds count={filter.OptionIds?.Count ?? 0}");

        var optionIds = filter.OptionIds?.Distinct().ToList();
        if (optionIds is null || optionIds.Count == 0)
            return [];

        var attributeId = new AttributeId(filter.AttributeId);

        var result = await _db.ProductAttributeValues
            .AsNoTracking()
            .Where(v => v.AttributeId == attributeId)
            .Where(v => v.OptionId.HasValue && optionIds.Contains(v.OptionId.Value))
            .Select(v => v.ProductId.Value)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Select filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetMultiSelectFilterProductIdsAsync(
        CatalogMultiSelectFilterDto filter,
        CancellationToken ct)
    {
        Console.WriteLine($"GetMultiSelectFilterProductIdsAsync: AttributeId={filter.AttributeId}, OptionIds count={filter.OptionIds?.Count ?? 0}");

        var optionIds = filter.OptionIds?.Distinct().ToList();
        if (optionIds is null || optionIds.Count == 0)
            return [];

        var attributeId = new AttributeId(filter.AttributeId);

        var result = await _db.ProductAttributeValues
            .AsNoTracking()
            .Where(v => v.AttributeId == attributeId)
            .Join(
                _db.ProductAttributeValueOptions.AsNoTracking().Where(o => optionIds.Contains(o.OptionId)),
                v => v.Id,
                o => EF.Property<Guid>(o, "product_attribute_value_id"),
                (v, o) => v.ProductId.Value)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Multi-select filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetBooleanFilterProductIdsAsync(
        CatalogBooleanFilterDto filter,
        CancellationToken ct)
    {
        Console.WriteLine($"GetBooleanFilterProductIdsAsync: AttributeId={filter.AttributeId}, Value={filter.Value}");

        var attributeId = new AttributeId(filter.AttributeId);

        var result = await _db.ProductAttributeValues
            .AsNoTracking()
            .Where(v => v.AttributeId == attributeId)
            .Where(v => v.BoolValue.HasValue && v.BoolValue.Value == filter.Value)
            .Select(v => v.ProductId.Value)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Boolean filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetNumberRangeFilterProductIdsAsync(
        CatalogNumberRangeFilterDto filter,
        CancellationToken ct)
    {
        Console.WriteLine($"GetNumberRangeFilterProductIdsAsync: AttributeId={filter.AttributeId}, Min={filter.Min}, Max={filter.Max}");

        var attributeId = new AttributeId(filter.AttributeId);

        var result = await _db.ProductAttributeValues
            .AsNoTracking()
            .Where(v => v.AttributeId == attributeId)
            .Where(v =>
                v.NumberValue.HasValue &&
                (!filter.Min.HasValue || v.NumberValue.Value >= filter.Min.Value) &&
                (!filter.Max.HasValue || v.NumberValue.Value <= filter.Max.Value))
            .Select(v => v.ProductId.Value)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Number range filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetTextFilterProductIdsAsync(
        CatalogTextFilterDto filter,
        CancellationToken ct)
    {
        var values = filter.Values?
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();

        Console.WriteLine($"GetTextFilterProductIdsAsync: AttributeId={filter.AttributeId}, Values count={values?.Count ?? 0}");

        if (values is null || values.Count == 0)
            return [];

        var attributeId = new AttributeId(filter.AttributeId);

        var result = await _db.ProductAttributeValues
            .AsNoTracking()
            .Where(v => v.AttributeId == attributeId)
            .Where(v => v.TextValue != null && values.Contains(v.TextValue))
            .Select(v => v.ProductId.Value)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Text filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetBrandFilterProductIdsAsync(
    IReadOnlyCollection<Guid> brandIds,
    CancellationToken ct)
    {
        Console.WriteLine($"GetBrandFilterProductIdsAsync: brandIds count={brandIds.Count}");

        if (brandIds.Count == 0)
            return [];

        var normalizedBrandIds = brandIds
            .Distinct()
            .ToHashSet();

        var rows = await _db.Products
            .AsNoTracking()
            .Where(x => x.Status == ProductStatus.Published)
            .Where(x => x.BrandId != null)
            .Select(x => new
            {
                ProductId = x.Id.Value,
                BrandId = x.BrandId
            })
            .ToListAsync(ct);

        Console.WriteLine($"Brand filter candidate rows: {rows.Count}");

        var result = rows
            .Where(x => x.BrandId is not null && normalizedBrandIds.Contains(x.BrandId.Value.Value))
            .Select(x => x.ProductId)
            .Distinct()
            .ToList();

        Console.WriteLine($"Brand filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetSellerGroupFilterProductIdsAsync(
        IReadOnlyCollection<string> sellerGroups,
        CancellationToken ct)
    {
        Console.WriteLine($"GetSellerGroupFilterProductIdsAsync: sellerGroups={string.Join(", ", sellerGroups)}");

        if (sellerGroups.Count == 0)
            return [];

        var normalized = sellerGroups
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        if (normalized.Count == 0)
            return [];

        var result = await _db.Offers
            .AsNoTracking()
            .Where(x => x.Status == OfferStatus.Active)
            .Join(
                _db.Sellers.AsNoTracking(),
                offer => offer.SellerId,
                seller => seller.Id,
                (offer, seller) => new
                {
                    ProductId = offer.ProductId.Value,
                    SellerType = seller.Type
                })
            .Where(x =>
                (normalized.Contains("platform") && x.SellerType == SellerType.Platform) ||
                (normalized.Contains("others") && x.SellerType == SellerType.Regular))
            .Select(x => x.ProductId)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Seller group filter found {result.Count} product ids.");
        return result;
    }

    private async Task<List<Guid>> GetPriceFilterProductIdsAsync(
        decimal? min,
        decimal? max,
        CancellationToken ct)
    {
        Console.WriteLine($"GetPriceFilterProductIdsAsync: min={min}, max={max}");

        var result = await _db.Offers
            .AsNoTracking()
            .Where(x => x.Status == OfferStatus.Active)
            .Where(x =>
                (!min.HasValue || x.Price.Amount >= min.Value) &&
                (!max.HasValue || x.Price.Amount <= max.Value))
            .Select(x => x.ProductId.Value)
            .Distinct()
            .ToListAsync(ct);

        Console.WriteLine($"Price filter found {result.Count} product ids.");
        return result;
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortBy)
    {
        var sortKey = sortBy?.Trim().ToLowerInvariant() ?? "newest";
        Console.WriteLine($"ApplySorting: sortBy='{sortBy}', resolved='{sortKey}'");

        return sortKey switch
        {
            "name_asc" => query.OrderBy(x => x.Name),
            "name_desc" => query.OrderByDescending(x => x.Name),
            "newest" => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }
}