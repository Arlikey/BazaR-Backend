namespace BazaR.Backend.Api.Contracts.Admin.Catalog.Categories.Attributes;

public sealed record AttachCategoryAttributeRequest(
    Guid AttributeId,
    bool IsRequired = false,
    bool IsFilterable = false,
    int SortOrder = 0,
    string? SectionName = null,
    int? SectionOrder = null
);
