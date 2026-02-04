using System.Linq;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products.Events;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 5000;

    // ===== Product Attributes (Entity inside aggregate) =====
    private readonly List<ProductAttributeValue> _attributeValues = new();
    public IReadOnlyCollection<ProductAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public CategoryId CategoryId { get; private set; }

    public BrandId? BrandId { get; private set; }
    public VendorCode? VendorCode { get; private set; }
    public ProductSlug? Slug { get; private set; }

    public ProductStatus Status { get; private set; }

    private Product(
        ProductId id,
        string name,
        CategoryId categoryId,
        BrandId? brandId,
        VendorCode? vendorCode,
        ProductSlug? slug
    ) : base(id)
    {
        Name = name;
        CategoryId = categoryId;
        BrandId = brandId;
        VendorCode = vendorCode;
        Slug = slug;

        Status = ProductStatus.Draft;
    }

    private Product() { } 

    public static Result<Product> Create(
        string name,
        CategoryId categoryId,
        BrandId? brandId = null,
        string? vendorCode = null,
        string? slug = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure(ProductErrors.NameRequired);

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
            return Result<Product>.Failure(ProductErrors.NameTooLong);

        if (categoryId == default)
            return Result<Product>.Failure(ProductErrors.CategoryRequired);

        VendorCode? vc = null;
        if (!string.IsNullOrWhiteSpace(vendorCode))
        {
            var vcRes = VendorCode.Create(vendorCode);
            if (vcRes.IsFailure) return Result<Product>.Failure(vcRes.Error);
            vc = vcRes.Value!;
        }

        ProductSlug? sl = null;
        if (!string.IsNullOrWhiteSpace(slug))
        {
            var slugRes = ProductSlug.Create(slug);
            if (slugRes.IsFailure) return Result<Product>.Failure(slugRes.Error);
            sl = slugRes.Value!;
        }

        var product = new Product(ProductId.New(), trimmedName, categoryId, brandId, vc, sl);
        product.AddDomainEvent(new ProductCreatedEvent(product.Id));

        return Result<Product>.Success(product);
    }

    // --------- Guards ----------
    private Result EnsureNotArchived()
        => Status == ProductStatus.Archived
            ? Result.Failure(ProductErrors.CannotModifyArchived)
            : Result.Success();

    // --------- Edit fields ----------
    public Result ChangeName(string name)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProductErrors.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength)
            return Result.Failure(ProductErrors.NameTooLong);

        Name = trimmed;
        return Result.Success();
    }

    public Result ChangeDescription(string? description)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (description is null)
        {
            Description = null;
            return Result.Success();
        }

        var trimmed = description.Trim();
        if (trimmed.Length > MaxDescriptionLength)
            return Result.Failure(ProductErrors.DescriptionTooLong);

        Description = trimmed.Length == 0 ? null : trimmed;
        return Result.Success();
    }

    public Result ChangeCategory(CategoryId categoryId)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (categoryId == default)
            return Result.Failure(ProductErrors.CategoryRequired);

        CategoryId = categoryId;
        return Result.Success();
    }

    public Result SetBrand(BrandId brandId)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        if (brandId.Value == default)
            return Result.Failure(ProductErrors.BrandRequired);

        BrandId = brandId;
        return Result.Success();
    }

    public Result SetVendorCode(string vendorCode)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        var vcRes = VendorCode.Create(vendorCode);
        if (vcRes.IsFailure)
            return Result.Failure(vcRes.Error);

        VendorCode = vcRes.Value!;
        return Result.Success();
    }

    public Result ClearVendorCode()
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        VendorCode = null;
        return Result.Success();
    }

    public Result SetSlug(string slug)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        var slugRes = ProductSlug.Create(slug);
        if (slugRes.IsFailure)
            return Result.Failure(slugRes.Error);

        Slug = slugRes.Value!;
        return Result.Success();
    }

    public Result ClearSlug()
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        Slug = null;
        return Result.Success();
    }

    // =========================================================
    // Product attribute values (характеристики товара)
    // =========================================================

    public Result SetAttributeValue(
        AttributeDefinition def,
        string? text = null,
        decimal? number = null,
        bool? boolean = null,
        Guid? optionId = null,
        IReadOnlyCollection<Guid>? optionIds = null)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        var existing = _attributeValues.SingleOrDefault(x => x.AttributeId == def.Id);
        if (existing is null)
        {
            existing = ProductAttributeValue.Create(def.Id);
            _attributeValues.Add(existing);
        }

        return existing.SetValue(def, text, number, boolean, optionId, optionIds);
    }

    public Result RemoveAttributeValue(BazaR.Backend.Domain.Catalog.Attributes.AttributeId attributeId)
    {
        var guard = EnsureNotArchived();
        if (guard.IsFailure) return guard;

        var existing = _attributeValues.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Success();

        _attributeValues.Remove(existing);
        return Result.Success();
    }

    // --------- Lifecycle ----------
    public Result Publish()
    {
        if (Status == ProductStatus.Archived)
            return Result.Failure(ProductErrors.CannotPublishArchived);

        if (Status == ProductStatus.Published)
            return Result.Success();

        Status = ProductStatus.Published;
        AddDomainEvent(new ProductPublishedEvent(Id));
        return Result.Success();
    }

    public Result Unpublish()
    {
        if (Status == ProductStatus.Archived)
            return Result.Failure(ProductErrors.CannotUnpublishArchived);

        if (Status == ProductStatus.Draft)
            return Result.Success();

        Status = ProductStatus.Draft;
        AddDomainEvent(new ProductUnpublishedEvent(Id));
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == ProductStatus.Archived)
            return Result.Success();

        Status = ProductStatus.Archived;
        AddDomainEvent(new ProductArchivedEvent(Id));
        return Result.Success();
    }
}
