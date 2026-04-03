namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed class CatalogFacetOptionDto
{
    public string Value { get; init; } = default!;
    public string Label { get; init; } = default!;
    public int Count { get; init; }
    public bool Selected { get; init; }
}