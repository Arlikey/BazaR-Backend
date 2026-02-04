using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog;

public static class ProductErrors
{
    public static readonly Error NameRequired =
        new("product.name.required", "Product name is required.", ErrorType.Validation);

    public static readonly Error NameTooLong =
        new("product.name.tooLong", "Product name is too long.", ErrorType.Validation);

    public static readonly Error DescriptionTooLong =
        new("product.description.tooLong", "Product description is too long.", ErrorType.Validation);

    public static readonly Error CategoryRequired =
        new("product.category.required", "Category is required.", ErrorType.Validation);

    public static readonly Error PriceMustBePositive =
        new("product.price.invalid", "Price must be greater than zero.", ErrorType.Validation);

    public static readonly Error CurrencyRequired =
        new("product.currency.required", "Currency is required.", ErrorType.Validation);

    public static readonly Error StockCannotBeNegative =
        new("product.stock.negative", "Stock cannot be negative.", ErrorType.Validation);

    public static readonly Error CannotActivateWithoutPrice =
        new("product.activate.noPrice", "Cannot activate product without price.", ErrorType.Conflict);

    public static readonly Error CannotActivateWithoutCategory =
        new("product.activate.noCategory", "Cannot activate product without category.", ErrorType.Conflict);

    public static readonly Error BrandRequired =
        new("Product.BrandRequired", "Brand is required.", ErrorType.Validation);

    public static readonly Error VendorCodeRequired =
        new("Product.VendorCodeRequired", "Vendor code is required.", ErrorType.Validation);

    public static readonly Error VendorCodeTooLong =
        new("Product.VendorCodeTooLong", "Vendor code is too long.", ErrorType.Validation);

    public static readonly Error VendorCodeInvalid =
        new("Product.VendorCodeInvalid", "Vendor code contains invalid characters.", ErrorType.Validation);

    public static readonly Error VendorCodeAlreadyExists =
        new("Product.VendorCodeAlreadyExists", "Product with this vendor code already exists.", ErrorType.Conflict);

    public static readonly Error SlugRequired =
        new("Product.SlugRequired", "Slug is required.", ErrorType.Validation);

    public static readonly Error SlugInvalid =
        new("Product.SlugInvalid", "Slug is invalid. Only lowercase letters, numbers and hyphens are allowed.", ErrorType.Validation);

    public static readonly Error SlugTooLong =
        new("Product.SlugTooLong", "Slug is too long.", ErrorType.Validation);

    public static readonly Error SlugAlreadyExists =
        new("Product.SlugAlreadyExists", "Product with this slug already exists.", ErrorType.Conflict);

    public static readonly Error NotFound =
        new("Product.NotFound", "Product not found.", ErrorType.NotFound);

    public static readonly Error CannotSetSelfAsParent =
        new("Product.CannotSetSelfAsParent", "Product cannot be set as parent of itself.", ErrorType.Validation);

    public static readonly Error CannotModifyArchived =
    new("Product.CannotModifyArchived", "Archived product cannot be modified.");

    public static readonly Error CannotPublishArchived =
        new("Product.CannotPublishArchived", "Cannot publish archived product.");

    public static readonly Error CannotUnpublishArchived =
        new("Product.CannotUnpublishArchived", "Cannot unpublish archived product.");

    public static readonly Error AttributeMismatch =
        new("ProductAttributeValue.AttributeMismatch", "AttributeDefinition does not match this ProductAttributeValue.AttributeId.");

    public static readonly Error DuplicateAttributeInRequest =
    new("Product.DuplicateAttributeInRequest", "Request contains duplicate attributes.");

    public static readonly Error AttributeNotAllowedForCategory =
        new("Product.AttributeNotAllowedForCategory", "Attribute is not allowed for this category.");

    public static readonly Error RequiredAttributesMissing =
        new("Product.RequiredAttributesMissing", "Some required attributes are missing.");


}