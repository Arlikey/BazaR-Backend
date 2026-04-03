namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed class CatalogFacetDto
{
    public string Kind { get; init; } = default!; // attribute / system

    public Guid? AttributeId { get; init; }
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Type { get; init; } = default!; // multi_select / range / boolean

    public string? Unit { get; init; }

    public string? SectionName { get; init; }
    public int? SectionOrder { get; init; }
    public int SortOrder { get; init; }

    public IReadOnlyList<CatalogFacetOptionDto> Options { get; init; } = [];
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
}