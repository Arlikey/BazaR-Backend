using BazaR.Backend.Application.Catalog.Browsing.DTOs;

namespace BazaR.Backend.Application.Catalog.Browsing.Sidebar.Abstractions;

public interface ICatalogSidebarReadRepository
{
    Task<CategoryCatalogSidebarDto> GetCategorySidebarAsync(
        Guid categoryId,
        CancellationToken ct = default);
}