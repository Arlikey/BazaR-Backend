namespace BazaR.Backend.Application.Catalog.Browsing.DTOs;

public sealed class AttributeFilterRequest
{
    public string AttributeCode { get; init; } = default!;
    public IReadOnlyList<string> Values { get; init; } = [];
    public decimal? NumberFrom { get; init; }
    public decimal? NumberTo { get; init; }
    public bool? BoolValue { get; init; }
}