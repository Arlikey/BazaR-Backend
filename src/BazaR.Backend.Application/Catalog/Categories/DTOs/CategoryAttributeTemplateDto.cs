using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetCategoryAttributeTemplate;

public sealed record CategoryAttributeTemplateSectionDto(
    string? SectionName,
    int? SectionOrder,
    IReadOnlyList<CategoryAttributeTemplateItemDto> Items);

public sealed record CategoryAttributeTemplateItemDto(
    Guid AttributeId,
    string Name,
    string Code,
    AttributeValueType ValueType,
    string? Unit,
    bool IsRequired,
    bool IsFilterable,
    int SortOrder,
    string? SectionName,
    int? SectionOrder);
