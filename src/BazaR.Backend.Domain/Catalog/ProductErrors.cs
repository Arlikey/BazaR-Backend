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
}
