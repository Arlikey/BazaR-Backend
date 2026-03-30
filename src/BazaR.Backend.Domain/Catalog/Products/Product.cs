using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products.Events;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using System.Linq;

namespace BazaR.Backend.Domain.Catalog.Products;

public sealed class Product : AggregateRoot<ProductId>
{
    // Ограничения длины полей
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 5000;
    private const int MaxHiddenReasonLength = 500;
    private const int MaxImages = 12; // максимум изображений на товар

    // Коллекции value objects (атрибуты и изображения)
    private readonly List<ProductAttributeValue> _attributeValues = new();
    public IReadOnlyCollection<ProductAttributeValue> AttributeValues => _attributeValues.AsReadOnly();

    private readonly List<ProductImage> _images = new();
    public IReadOnlyCollection<ProductImage> Images => _images.AsReadOnly();

    // Владелец (продавец)
    public SellerId OwnerSellerId { get; private set; } = default!;

    // Основные характеристики
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public BrandId? BrandId { get; private set; }
    public ProductSlug? Slug { get; private set; }

    // Идентификаторы
    public VendorCode? VendorCode { get; private set; }   // артикул продавца
    public ProductBarcode? Barcode { get; private set; }  // EAN/UPC

    // Видимость и модерация
    public ProductStatus Status { get; private set; }
    public DateTimeOffset? HiddenAt { get; private set; }
    public Guid? HiddenBy { get; private set; }
    public string? HiddenReason { get; private set; }

    // Аудит
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // Приватный конструктор для фабрики
    private Product(
        ProductId id,
        SellerId ownerSellerId,
        string name,
        string? description,
        CategoryId categoryId,
        BrandId? brandId,
        ProductSlug? slug,
        VendorCode? vendorCode,
        ProductBarcode? barcode,
        DateTimeOffset nowUtc
    ) : base(id)
    {
        OwnerSellerId = ownerSellerId;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        BrandId = brandId;
        Slug = slug;
        VendorCode = vendorCode;
        Barcode = barcode;
        Status = ProductStatus.Draft;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
        AddDomainEvent(new ProductCreatedEvent(Id));
    }

    private Product() { } 

    // =========================================================
    // Factory
    // =========================================================
    public static Result<Product> CreateBySeller(
        SellerId ownerSellerId,
        string name,
        CategoryId categoryId,
        BrandId? brandId = null,
        string? description = null,
        string? slug = null,
        string? vendorCode = null,
        string? barcode = null)
    {
        // Валидация входных данных
        if (ownerSellerId.Value == default)
            return Result<Product>.Failure(ProductErrors.OwnerSellerRequired);

        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure(ProductErrors.NameRequired);

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
            return Result<Product>.Failure(ProductErrors.NameTooLong);

        var desc = Normalize(description);
        if (desc is not null && desc.Length > MaxDescriptionLength)
            return Result<Product>.Failure(ProductErrors.DescriptionTooLong);

        if (categoryId.Value == default)
            return Result<Product>.Failure(ProductErrors.CategoryRequired);

        if (brandId is not null && brandId.Value == default)
            return Result<Product>.Failure(ProductErrors.BrandRequired);

        // Создание value objects (если переданы)
        ProductSlug? sl = null;
        if (!string.IsNullOrWhiteSpace(slug))
        {
            var slugRes = ProductSlug.Create(slug);
            if (slugRes.IsFailure) return Result<Product>.Failure(slugRes.Error);
            sl = slugRes.Value!;
        }

        VendorCode? vc = null;
        if (!string.IsNullOrWhiteSpace(vendorCode))
        {
            var vcRes = VendorCode.Create(vendorCode);
            if (vcRes.IsFailure) return Result<Product>.Failure(vcRes.Error);
            vc = vcRes.Value!;
        }

        ProductBarcode? bc = null;
        if (!string.IsNullOrWhiteSpace(barcode))
        {
            var bcRes = ProductBarcode.Create(barcode);
            if (bcRes.IsFailure) return Result<Product>.Failure(bcRes.Error);
            bc = bcRes.Value!;
        }

        var now = DateTimeOffset.UtcNow;

        return Result<Product>.Success(
            new Product(ProductId.New(), ownerSellerId, trimmedName, desc, categoryId, brandId, sl, vc, bc, now));
    }

    // =========================================================
    // Guards
    // =========================================================
    private Result EnsureEditable()
        => Status switch
        {
            ProductStatus.Archived => Result.Failure(ProductErrors.CannotModifyArchived),
            ProductStatus.Hidden => Result.Failure(ProductErrors.CannotModifyHidden),
            _ => Result.Success()
        };

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    // =========================================================
    // Update basics (seller)
    // =========================================================
    public Result UpdateBasics(
        string name,
        string? description,
        CategoryId categoryId,
        BrandId? brandId,
        string? slug)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        // Валидация
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(ProductErrors.NameRequired);

        var trimmedName = name.Trim();
        if (trimmedName.Length > MaxNameLength)
            return Result.Failure(ProductErrors.NameTooLong);

        var desc = Normalize(description);
        if (desc is not null && desc.Length > MaxDescriptionLength)
            return Result.Failure(ProductErrors.DescriptionTooLong);

        if (categoryId.Value == default)
            return Result.Failure(ProductErrors.CategoryRequired);

        if (brandId is not null && brandId.Value == default)
            return Result.Failure(ProductErrors.BrandRequired);

        ProductSlug? sl = null;
        if (!string.IsNullOrWhiteSpace(slug))
        {
            var slugRes = ProductSlug.Create(slug);
            if (slugRes.IsFailure) return Result.Failure(slugRes.Error);
            sl = slugRes.Value!;
        }

        // Применяем изменения
        Name = trimmedName;
        Description = desc;
        CategoryId = categoryId;
        BrandId = brandId;
        Slug = sl;

        Touch();
        AddDomainEvent(new ProductUpdatedEvent(Id));
        return Result.Success();
    }

    // =========================================================
    // Publish / Unpublish
    // =========================================================
    public Result Publish()
    {
        if (Status == ProductStatus.Archived)
            return Result.Failure(ProductErrors.CannotPublishArchived);
        if (Status == ProductStatus.Hidden)
            return Result.Failure(ProductErrors.CannotPublishHidden);
        if (Status == ProductStatus.Published)
            return Result.Success();

        // Проверка минимальной готовности
        if (string.IsNullOrWhiteSpace(Name))
            return Result.Failure(ProductErrors.NameRequired);
        if (CategoryId.Value == default)
            return Result.Failure(ProductErrors.CategoryRequired);

        Status = ProductStatus.Published;
        Touch();
        AddDomainEvent(new ProductPublishedEvent(Id));
        return Result.Success();
    }

    public Result Unpublish()
    {
        if (Status == ProductStatus.Archived)
            return Result.Failure(ProductErrors.CannotUnpublishArchived);
        if (Status == ProductStatus.Hidden)
            return Result.Failure(ProductErrors.CannotUnpublishHidden);
        if (Status == ProductStatus.Draft)
            return Result.Success();

        Status = ProductStatus.Draft;
        Touch();
        AddDomainEvent(new ProductUnpublishedEvent(Id));
        return Result.Success();
    }

    // =========================================================
    // VendorCode
    // =========================================================
    public Result SetVendorCode(string? vendorCode)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        if (string.IsNullOrWhiteSpace(vendorCode))
        {
            if (VendorCode is null) return Result.Success();
            VendorCode = null;
            Touch();
            AddDomainEvent(new ProductVendorCodeClearedEvent(Id));
            return Result.Success();
        }

        var vcRes = VendorCode.Create(vendorCode);
        if (vcRes.IsFailure)
            return Result.Failure(vcRes.Error);

        var newVc = vcRes.Value!;
        if (VendorCode is not null && VendorCode.Equals(newVc))
            return Result.Success();

        VendorCode = newVc;
        Touch();
        AddDomainEvent(new ProductVendorCodeChangedEvent(Id, VendorCode.Value));
        return Result.Success();
    }

    // =========================================================
    // Barcode
    // =========================================================
    public Result SetBarcode(string? barcode)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        if (string.IsNullOrWhiteSpace(barcode))
        {
            if (Barcode is null) return Result.Success();
            Barcode = null;
            Touch();
            AddDomainEvent(new ProductBarcodeClearedEvent(Id));
            return Result.Success();
        }

        var bcRes = ProductBarcode.Create(barcode);
        if (bcRes.IsFailure)
            return Result.Failure(bcRes.Error);

        var newBc = bcRes.Value!;
        if (Barcode is not null && Barcode.Equals(newBc))
            return Result.Success();

        Barcode = newBc;
        Touch();
        AddDomainEvent(new ProductBarcodeChangedEvent(Id, Barcode.Value));
        return Result.Success();
    }

    // =========================================================
    // Moderation (admin)
    // =========================================================
    public Result Hide(Guid adminUserId, string? reason)
    {
        if (Status == ProductStatus.Archived)
            return Result.Failure(ProductErrors.CannotHideArchived);
        if (adminUserId == Guid.Empty)
            return Result.Failure(ProductErrors.AdminRequired);

        var r = Normalize(reason);
        if (r is not null && r.Length > MaxHiddenReasonLength)
            return Result.Failure(ProductErrors.HiddenReasonTooLong);

        if (Status == ProductStatus.Hidden && HiddenReason == r)
            return Result.Success();

        Status = ProductStatus.Hidden;
        HiddenAt = DateTimeOffset.UtcNow;
        HiddenBy = adminUserId;
        HiddenReason = r;

        Touch();
        AddDomainEvent(new ProductHiddenEvent(Id, adminUserId, r));
        return Result.Success();
    }

    public Result Unhide(Guid adminUserId)
    {
        if (adminUserId == Guid.Empty)
            return Result.Failure(ProductErrors.AdminRequired);
        if (Status != ProductStatus.Hidden)
            return Result.Success();

        Status = ProductStatus.Draft;
        HiddenAt = null;
        HiddenBy = null;
        HiddenReason = null;

        Touch();
        AddDomainEvent(new ProductUnhiddenEvent(Id, adminUserId));
        return Result.Success();
    }

    // =========================================================
    // Owner action
    // =========================================================
    public Result ArchiveByOwner()
    {
        if (Status == ProductStatus.Archived)
            return Result.Success();

        Status = ProductStatus.Archived;
        Touch();
        AddDomainEvent(new ProductArchivedEvent(Id));
        return Result.Success();
    }

    // =========================================================
    // Attributes
    // =========================================================
    public Result SetAttributeValue(
        AttributeDefinition def,
        string? text = null,
        decimal? number = null,
        bool? boolean = null,
        Guid? optionId = null,
        IReadOnlyCollection<Guid>? optionIds = null)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        var existing = _attributeValues.SingleOrDefault(x => x.AttributeId == def.Id);
        if (existing is null)
        {
            existing = ProductAttributeValue.Create(def.Id);
            _attributeValues.Add(existing);
        }

        var res = existing.SetValue(def, text, number, boolean, optionId, optionIds);
        if (res.IsFailure) return res;

        Touch();
        AddDomainEvent(new ProductAttributeValueSetEvent(Id, def.Id));
        return Result.Success();
    }

    public Result RemoveAttributeValue(AttributeId attributeId)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        var existing = _attributeValues.SingleOrDefault(x => x.AttributeId == attributeId);
        if (existing is null)
            return Result.Success();

        _attributeValues.Remove(existing);
        Touch();
        AddDomainEvent(new ProductAttributeValueRemovedEvent(Id, attributeId));
        return Result.Success();
    }

    // =========================================================
    // Images
    // =========================================================
    public Result<Guid> AddImage(
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        bool makeMain = false)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return Result<Guid>.Failure(g.Error);

        // Базовая валидация
        if (string.IsNullOrWhiteSpace(url))
            return Result<Guid>.Failure(ProductErrors.UrlRequired);
        if (string.IsNullOrWhiteSpace(storageKey))
            return Result<Guid>.Failure(ProductErrors.StorageKeyRequired);
        if (string.IsNullOrWhiteSpace(contentType))
            return Result<Guid>.Failure(ProductErrors.ContentTypeRequired);
        if (sizeBytes <= 0)
            return Result<Guid>.Failure(ProductErrors.InvalidSize);
        if (_images.Count >= MaxImages)
            return Result<Guid>.Failure(ProductErrors.TooManyImages);

        var now = DateTimeOffset.UtcNow;
        var nextSort = _images.Count == 0 ? 0 : _images.Max(x => x.SortOrder) + 1;

        var setMain = makeMain || _images.Count == 0;
        if (setMain)
        {
            foreach (var img in _images)
                img.SetMain(false);
        }

        var image = ProductImage.Create(
            productId: Id,
            url: url.Trim(),
            storageKey: storageKey.Trim(),
            contentType: contentType.Trim(),
            sizeBytes: sizeBytes,
            isMain: setMain,
            sortOrder: nextSort,
            nowUtc: now);

        _images.Add(image);
        Touch();
        return Result<Guid>.Success(image.Id);
    }

    public Result RemoveImage(Guid imageId)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        var img = _images.SingleOrDefault(x => x.Id == imageId);
        if (img is null)
            return Result.Failure(ProductErrors.ImageNotFound);

        var wasMain = img.IsMain;
        _images.Remove(img);

        // Если удалили главное, назначаем новое первое по порядку
        if (wasMain && _images.Count > 0)
        {
            var next = _images.OrderBy(x => x.SortOrder).First();
            foreach (var i in _images)
                i.SetMain(i.Id == next.Id);
        }

        Touch();
        return Result.Success();
    }

    public Result SetMainImage(Guid imageId)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        var img = _images.SingleOrDefault(x => x.Id == imageId);
        if (img is null)
            return Result.Failure(ProductErrors.ImageNotFound);

        if (img.IsMain)
            return Result.Success();

        // Сбрасываем флаг у всех, ставим только у выбранного
        foreach (var i in _images)
            i.SetMain(i.Id == imageId);

        Touch();
        return Result.Success();
    }

    public Result ReorderImages(IReadOnlyList<Guid> orderedImageIds)
    {
        var g = EnsureEditable();
        if (g.IsFailure) return g;

        if (orderedImageIds is null || orderedImageIds.Count == 0)
            return Result.Success();

        var set = _images.Select(x => x.Id).ToHashSet();
        if (orderedImageIds.Any(id => !set.Contains(id)))
            return Result.Failure(ProductErrors.SortOrderInvalid);

        for (var i = 0; i < orderedImageIds.Count; i++)
        {
            var id = orderedImageIds[i];
            var img = _images.Single(x => x.Id == id);
            img.SetSortOrder(i);
        }

        Touch();
        return Result.Success();
    }


    public Result SetBrand(BrandId? brandId)
    {
        BrandId = brandId;
        return Result.Success();
    }


    public string? GetMainImageUrl()
        => _images
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.SortOrder)
            .Select(x => x.Url)
            .FirstOrDefault();
}