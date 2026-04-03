namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed class CategoryCatalogSidebarRequest
{
    public Guid CategoryId { get; init; }
    public IReadOnlyList<AttributeFilterRequest> Attributes { get; init; } = [];
    public SystemFilterRequest System { get; init; } = new();
}