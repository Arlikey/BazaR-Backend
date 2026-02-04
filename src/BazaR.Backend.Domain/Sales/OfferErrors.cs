using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sales;

public static class OfferErrors
{
    public static readonly Error ProductRequired =
        new("Offer.ProductRequired", "Product is required.");

    public static readonly Error SellerRequired =
        new("Offer.SellerRequired", "Seller is required.");

    public static readonly Error CannotActivateWithoutPrice =
        new("Offer.CannotActivateWithoutPrice", "Cannot activate offer without price.");

    public static readonly Error CannotActivateWithoutStock =
        new("Offer.CannotActivateWithoutStock", "Cannot activate offer without stock.");

    public static readonly Error StockCannotBeNegative =
        new("Offer.StockCannotBeNegative", "Stock cannot be negative.");

    public static readonly Error AmountMustBePositive =
        new("Offer.AmountMustBePositive", "Amount must be positive.");

    public static readonly Error ArchivedCannotBeModified =
        new("Offer.ArchivedCannotBeModified", "Offer is archived and cannot be modified.");
    public static readonly Error CannotActivateArchived =
    new("Offer.CannotActivateArchived", "Cannot activate archived offer.");

}
