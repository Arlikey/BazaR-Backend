namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public abstract record CatalogSelectedFilterDto
{
    public required Guid AttributeId { get; init; }
}

public sealed record CatalogSelectFilterDto : CatalogSelectedFilterDto
{
    public required IReadOnlyCollection<Guid> OptionIds { get; init; }
}

public sealed record CatalogMultiSelectFilterDto : CatalogSelectedFilterDto
{
    public required IReadOnlyCollection<Guid> OptionIds { get; init; }
}

public sealed record CatalogBooleanFilterDto : CatalogSelectedFilterDto
{
    public required bool Value { get; init; }
}

public sealed record CatalogNumberRangeFilterDto : CatalogSelectedFilterDto
{
    public decimal? Min { get; init; }
    public decimal? Max { get; init; }
}

public sealed record CatalogTextFilterDto : CatalogSelectedFilterDto
{
    public required IReadOnlyCollection<string> Values { get; init; }
}