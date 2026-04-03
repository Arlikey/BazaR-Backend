namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed class SystemFilterRequest
{
    public IReadOnlyList<Guid> BrandIds { get; init; } = [];
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public bool? InStock { get; init; }
}