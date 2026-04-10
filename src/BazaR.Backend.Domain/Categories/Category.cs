using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Categories.Events;

namespace BazaR.Backend.Domain.Categories;

public sealed class Category : AggregateRoot<CategoryId>
{
    private const int MaxNameLength = 200;

    private readonly List<CategoryAttribute> _attributes = new();
    public IReadOnlyCollection<CategoryAttribute> Attributes => _attributes.AsReadOnly();

    private CategoryImage? _image;
    public CategoryImage? Image => _image;

    public CategorySlug? Slug { get; private set; }
    public string Name { get; private set; } = default!;
    public CategoryId? ParentCategoryId { get; private set; }
    public int SortOrder { get; private set; }

    private Category(CategoryId id, string name, CategoryId? parentCategoryId, int sortOrder)
        : base(id)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
        SortOrder = sortOrder;
    }

    private Category() { }

    public static Result<Category> Create(
        string name,
        CategoryId? parentCategoryId = null,
        int sortOrder = 0,
        CategorySlug? slug = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Category>.Failure(CategoryErrors.NameRequired);

        var trimmed = name.Trim();

        if (trimmed.Length > MaxNameLength)
            return Result<Category>.Failure(CategoryErrors.NameTooLong);

        if (sortOrder < 0)
            return Result<Category>.Failure(CategoryErrors.SortOrderCannotBeNegative);

        var category = new Category(CategoryId.New(), trimmed, parentCategoryId, sortOrder)
        {
            Slug = slug
        };

        category.AddDomainEvent(new CategoryCreatedEvent(category.Id));

        return Result<Category>.Success(category);
    }

    public Result SetSlug(CategorySlug? slug)
    {
        if (Slug == slug)
            return Result.Success();

        Slug = slug;
        AddDomainEvent(new CategorySlugChangedEvent(Id, Slug?.Value));

        return Result.Success();
    }

    public Result Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(CategoryErrors.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength)
            return Result.Failure(CategoryErrors.NameTooLong);

        if (Name == trimmed)
            return Result.Success();

        Name = trimmed;
        AddDomainEvent(new CategoryRenamedEvent(Id, Name));
        return Result.Success();
    }

    public Result Move(CategoryId? newParentCategoryId, int newSortOrder = 0)
    {
        if (newParentCategoryId.HasValue && newParentCategoryId.Value == Id)
            return Result.Failure(CategoryErrors.CannotSetSelfAsParent);

        if (newSortOrder < 0)
            return Result.Failure(CategoryErrors.SortOrderCannotBeNegative);

        if (ParentCategoryId == newParentCategoryId && SortOrder == newSortOrder)
            return Result.Success();

        ParentCategoryId = newParentCategoryId;
        SortOrder = newSortOrder;

        AddDomainEvent(new CategoryParentChangedEvent(Id, ParentCategoryId, SortOrder));
        return Result.Success();
    }

    // ========== Attributes ==========

    public Result AddAttribute(
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
        if (_attributes.Any(x => x.AttributeId == attributeId))
            return Result.Failure(CategoryAttributeErrors.DuplicateAttributeInCategory);

        var created = CategoryAttribute.Create(
            attributeId,
            isRequired,
            isFilterable,
            filterPresentationType,
            isVisibleInSpecifications,
            isVisibleOnProductCard,
            sortOrder,
            sectionName,
            sectionOrder);

        if (created.IsFailure)
            return Result.Failure(created.Error);

        _attributes.Add(created.Value!);
        return Result.Success();
    }

    public Result RemoveAttribute(AttributeId attributeId)
    {
        var existing = _attributes.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Success();

        _attributes.Remove(existing);
        return Result.Success();
    }

    public Result UpdateAttributeRules(
        AttributeId attributeId,
        bool isRequired,
        bool isFilterable,
        FilterPresentationType? filterPresentationType)
    {
        var existing = _attributes.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Failure(CategoryErrors.NotFound);

        return existing.UpdateRules(isRequired, isFilterable, filterPresentationType);
    }

    public Result SetAttributeVisibility(
        AttributeId attributeId,
        bool isVisibleInSpecifications,
        bool isVisibleOnProductCard)
    {
        var existing = _attributes.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Failure(CategoryErrors.NotFound);

        return existing.SetVisibility(isVisibleInSpecifications, isVisibleOnProductCard);
    }

    public Result SetAttributeSortOrder(AttributeId attributeId, int sortOrder)
    {
        var existing = _attributes.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Failure(CategoryErrors.NotFound);

        return existing.SetSortOrder(sortOrder);
    }

    public Result SetAttributeSection(AttributeId attributeId, string? sectionName, int? sectionOrder = null)
    {
        var existing = _attributes.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Failure(CategoryErrors.NotFound);

        return existing.SetSection(sectionName, sectionOrder);
    }

    // ========== Image ==========

    public Result SetImage(string url, string storageKey, string contentType, long sizeBytes)
    {
        var now = DateTimeOffset.UtcNow;

        var created = CategoryImage.Create(url, storageKey, contentType, sizeBytes, now);
        if (created.IsFailure)
            return Result.Failure(created.Error);

        if (_image is null)
        {
            _image = created.Value;
        }
        else
        {
            _image.Replace(url, storageKey, contentType, sizeBytes, now);
        }

        return Result.Success();
    }

    public Result RemoveImage()
    {
        if (_image is null)
            return Result.Success();

        _image = null;
        return Result.Success();
    }
}