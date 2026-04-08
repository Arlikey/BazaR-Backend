namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed record CatalogSystemFiltersDto
{
    public IReadOnlyCollection<Guid> BrandIds { get; init; } = [];
    public IReadOnlyCollection<string> SellerGroups { get; init; } = [];
    public decimal? PriceMin { get; init; }
    public decimal? PriceMax { get; init; }
}