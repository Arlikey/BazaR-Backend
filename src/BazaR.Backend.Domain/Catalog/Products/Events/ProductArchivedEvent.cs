using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products.Events;

public sealed record ProductArchivedEvent(ProductId ProductId) : DomainEvent;


public sealed record ProductNameChangedEvent(ProductId ProductId, string Name) : DomainEvent;
public sealed record ProductDescriptionChangedEvent(ProductId ProductId) : DomainEvent;
public sealed record ProductCategoryChangedEvent(ProductId ProductId, CategoryId CategoryId) : DomainEvent;
public sealed record ProductBrandChangedEvent(ProductId ProductId, BrandId? BrandId) : DomainEvent;

public sealed record ProductVendorCodeChangedEvent(ProductId ProductId, string VendorCode) : DomainEvent;
public sealed record ProductVendorCodeClearedEvent(ProductId ProductId) : DomainEvent;

public sealed record ProductSlugChangedEvent(ProductId ProductId, string Slug) : DomainEvent;
public sealed record ProductSlugClearedEvent(ProductId ProductId) : DomainEvent;

public sealed record ProductAttributeValueSetEvent(ProductId ProductId, AttributeId AttributeId) : DomainEvent;
public sealed record ProductAttributeValueRemovedEvent(ProductId ProductId, AttributeId AttributeId) : DomainEvent;


public sealed record ProductUpdatedEvent(ProductId ProductId) : DomainEvent;

public sealed record ProductPublishedEvent(ProductId ProductId) : DomainEvent;

public sealed record ProductBarcodeChangedEvent(ProductId ProductId, string Barcode) : DomainEvent;
public sealed record ProductBarcodeClearedEvent(ProductId ProductId) : DomainEvent;

public sealed record ProductMainImageChangedEvent(ProductId ProductId, string? MainImageUrl) : DomainEvent;

public sealed record ProductHiddenEvent(ProductId ProductId, Guid AdminUserId, string? Reason) : DomainEvent;
public sealed record ProductUnhiddenEvent(ProductId ProductId, Guid AdminUserId) : DomainEvent;


public sealed record ProductImageAddedEvent(ProductId ProductId, Guid ImageId) : DomainEvent;
public sealed record ProductImageRemovedEvent(ProductId ProductId, Guid ImageId) : DomainEvent;
