namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed class CategoryCatalogSidebarDto
{
    public Guid CategoryId { get; init; }
    public IReadOnlyList<CatalogFacetDto> Facets { get; init; } = [];
}