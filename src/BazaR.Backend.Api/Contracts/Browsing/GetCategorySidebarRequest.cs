namespace BazaR.Backend.Api.Contracts.Browsing;

public sealed class GetCategorySidebarRequest
{
    public IReadOnlyList<AttributeSidebarFilterRequest> Attributes { get; init; } = [];
    public SidebarSystemFilterRequest System { get; init; } = new();
}

public sealed class AttributeSidebarFilterRequest
{
    public string AttributeCode { get; init; } = default!;
    public IReadOnlyList<string> Values { get; init; } = [];
    public decimal? NumberFrom { get; init; }
    public decimal? NumberTo { get; init; }
    public bool? BoolValue { get; init; }
}

public sealed class SidebarSystemFilterRequest
{
    public IReadOnlyList<Guid> BrandIds { get; init; } = [];
    public decimal? PriceFrom { get; init; }
    public decimal? PriceTo { get; init; }
    public bool? InStock { get; init; }
}