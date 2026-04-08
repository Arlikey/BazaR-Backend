using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Application.Sellers.DTOs;

namespace BazaR.Backend.Application.Catalog.Browsing.Abstractions;

public interface ICatalogBrowseReadRepository
{
    //Task<CategoryCatalogSidebarDto> GetCategorySidebarAsync(Guid categoryId, CancellationToken ct = default);

    Task<PagedResult<ProductCardDto>> BrowseCategoryProductsAsync(
     Guid categoryId,
     IReadOnlyCollection<CatalogSelectedFilterDto> filters,
     CatalogSystemFiltersDto? systemFilters,
     int page,
     int pageSize,
     string? sortBy,
     CancellationToken ct = default);
}