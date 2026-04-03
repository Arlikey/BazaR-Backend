using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Categories;

public sealed class CategoryAttribute : Entity<CategoryAttributeId>
{
    private const int MaxSectionNameLength = 100;

    public AttributeId AttributeId { get; private set; }

    public bool IsRequired { get; private set; }
    public bool IsFilterable { get; private set; }

    public FilterPresentationType? FilterPresentationType { get; private set; }

    public bool IsVisibleInSpecifications { get; private set; }
    public bool IsVisibleOnProductCard { get; private set; }

    public int SortOrder { get; private set; }

    public string? SectionName { get; private set; }
    public int? SectionOrder { get; private set; }

    private CategoryAttribute(
        CategoryAttributeId id,
        AttributeId attributeId,
        bool isRequired,
        bool isFilterable,
        FilterPresentationType? filterPresentationType,
        bool isVisibleInSpecifications,
        bool isVisibleOnProductCard,
        int sortOrder,
        string? sectionName,
        int? sectionOrder)
        : base(id)
    {
        AttributeId = attributeId;
        IsRequired = isRequired;
        IsFilterable = isFilterable;
        FilterPresentationType = filterPresentationType;
        IsVisibleInSpecifications = isVisibleInSpecifications;
        IsVisibleOnProductCard = isVisibleOnProductCard;
        SortOrder = sortOrder;
        SectionName = sectionName;
        SectionOrder = sectionOrder;
    }

    private CategoryAttribute() { }

    internal static Result<CategoryAttribute> Create(
        AttributeId attributeId,
        bool isRequired = false,
        bool isFilterable = false,
        FilterPresentationType? filterPresentationType = null,
        bool isVisibleInSpecifications = true,
        bool isVisibleOnProductCard = false,
        int sortOrder = 0,
        string? sectionName = null,
        int? sectionOrder = null)
    {
        if (sortOrder < 0)
            return Result<CategoryAttribute>.Failure(CategoryAttributeErrors.SortOrderCannotBeNegative);

        var normalizedSectionName = NormalizeSectionName(sectionName);
        if (normalizedSectionName is not null && normalizedSectionName.Length > MaxSectionNameLength)
            return Result<CategoryAttribute>.Failure(CategoryAttributeErrors.SectionNameTooLong);

        if (sectionOrder is not null && sectionOrder.Value < 0)
            return Result<CategoryAttribute>.Failure(CategoryAttributeErrors.SectionOrderCannotBeNegative);

        if (!isFilterable && filterPresentationType is not null)
        {
            return Result<CategoryAttribute>.Failure(new Error(
                "CategoryAttribute.FilterPresentationType.NotAllowed",
                "Filter presentation type can be set only for filterable attributes."));
        }

        if (normalizedSectionName is null)
            sectionOrder = null;

        return Result<CategoryAttribute>.Success(
            new CategoryAttribute(
                CategoryAttributeId.New(),
                attributeId,
                isRequired,
                isFilterable,
                filterPresentationType,
                isVisibleInSpecifications,
                isVisibleOnProductCard,
                sortOrder,
                normalizedSectionName,
                sectionOrder));
    }

    internal Result UpdateRules(
        bool isRequired,
        bool isFilterable,
        FilterPresentationType? filterPresentationType)
    {
        if (!isFilterable && filterPresentationType is not null)
        {
            return Result.Failure(new Error(
                "CategoryAttribute.FilterPresentationType.NotAllowed",
                "Filter presentation type can be set only for filterable attributes."));
        }

        if (IsRequired == isRequired &&
            IsFilterable == isFilterable &&
            FilterPresentationType == filterPresentationType)
        {
            return Result.Success();
        }

        IsRequired = isRequired;
        IsFilterable = isFilterable;
        FilterPresentationType = isFilterable ? filterPresentationType : null;

        return Result.Success();
    }

    internal Result SetFilterPresentationType(FilterPresentationType? filterPresentationType)
    {
        if (!IsFilterable && filterPresentationType is not null)
        {
            return Result.Failure(new Error(
                "CategoryAttribute.FilterPresentationType.NotAllowed",
                "Filter presentation type can be set only for filterable attributes."));
        }

        if (FilterPresentationType == filterPresentationType)
            return Result.Success();

        FilterPresentationType = filterPresentationType;
        return Result.Success();
    }

    internal Result SetVisibility(
        bool isVisibleInSpecifications,
        bool isVisibleOnProductCard)
    {
        if (IsVisibleInSpecifications == isVisibleInSpecifications &&
            IsVisibleOnProductCard == isVisibleOnProductCard)
        {
            return Result.Success();
        }

        IsVisibleInSpecifications = isVisibleInSpecifications;
        IsVisibleOnProductCard = isVisibleOnProductCard;
        return Result.Success();
    }

    internal Result SetSortOrder(int sortOrder)
    {
        if (sortOrder < 0)
            return Result.Failure(CategoryAttributeErrors.SortOrderCannotBeNegative);

        if (SortOrder == sortOrder)
            return Result.Success();

        SortOrder = sortOrder;
        return Result.Success();
    }

    internal Result SetSection(string? sectionName, int? sectionOrder = null)
    {
        var normalized = NormalizeSectionName(sectionName);

        if (normalized is not null && normalized.Length > MaxSectionNameLength)
            return Result.Failure(CategoryAttributeErrors.SectionNameTooLong);

        if (sectionOrder is not null && sectionOrder.Value < 0)
            return Result.Failure(CategoryAttributeErrors.SectionOrderCannotBeNegative);

        if (normalized is null)
            sectionOrder = null;

        if (SectionName == normalized && SectionOrder == sectionOrder)
            return Result.Success();

        SectionName = normalized;
        SectionOrder = sectionOrder;
        return Result.Success();
    }

    private static string? NormalizeSectionName(string? name)
        => string.IsNullOrWhiteSpace(name) ? null : name.Trim();
}