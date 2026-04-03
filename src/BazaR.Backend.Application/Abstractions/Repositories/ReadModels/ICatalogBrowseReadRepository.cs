using BazaR.Backend.Application.Catalog.Browsing.DTOs;

namespace BazaR.Backend.Application.Catalog.Browsing.Abstractions;

public interface ICatalogBrowseReadRepository
{
    Task<CategoryCatalogSidebarDto> GetCategorySidebarAsync(Guid categoryId, CancellationToken ct = default);
}