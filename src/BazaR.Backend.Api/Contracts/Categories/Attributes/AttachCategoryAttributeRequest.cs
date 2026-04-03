using BazaR.Backend.Domain.Categories;

namespace BazaR.Backend.Api.Contracts.Categories.Attributes;

public sealed class AttachCategoryAttributeRequest
{
    public Guid AttributeId { get; init; }
    public bool IsRequired { get; init; }
    public bool IsFilterable { get; init; }
    public FilterPresentationType? FilterPresentationType { get; init; }
    public bool IsVisibleInSpecifications { get; init; } = true;
    public bool IsVisibleOnProductCard { get; init; } = false;
    public int SortOrder { get; init; }
    public string? SectionName { get; init; }
    public int? SectionOrder { get; init; }
}