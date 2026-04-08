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


/*public sealed class BrowseCategoryProductsRequest
{
    public List<SelectFilterRequest> SelectFilters { get; set; } = new();
    public List<MultiSelectFilterRequest> MultiSelectFilters { get; set; } = new();
    public List<BooleanFilterRequest> BooleanFilters { get; set; } = new();
    public List<NumberRangeFilterRequest> NumberRangeFilters { get; set; } = new();
    public List<TextFilterRequest> TextFilters { get; set; } = new();

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
}*/

public sealed class BrowseCategoryProductsRequest
{
    public List<SelectFilterRequest> SelectFilters { get; set; } = new();
    public List<MultiSelectFilterRequest> MultiSelectFilters { get; set; } = new();
    public List<BooleanFilterRequest> BooleanFilters { get; set; } = new();
    public List<NumberRangeFilterRequest> NumberRangeFilters { get; set; } = new();
    public List<TextFilterRequest> TextFilters { get; set; } = new();

    public List<Guid> BrandIds { get; set; } = new();
    public List<string> SellerGroups { get; set; } = new();
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
}

public sealed class SelectFilterRequest
{
    public Guid AttributeId { get; set; }
    public List<Guid> OptionIds { get; set; } = new();
}

public sealed class MultiSelectFilterRequest
{
    public Guid AttributeId { get; set; }
    public List<Guid> OptionIds { get; set; } = new();
}

public sealed class BooleanFilterRequest
{
    public Guid AttributeId { get; set; }
    public bool Value { get; set; }
}

public sealed class NumberRangeFilterRequest
{
    public Guid AttributeId { get; set; }
    public decimal? Min { get; set; }
    public decimal? Max { get; set; }
}

public sealed class TextFilterRequest
{
    public Guid AttributeId { get; set; }
    public List<string> Values { get; set; } = new();
}