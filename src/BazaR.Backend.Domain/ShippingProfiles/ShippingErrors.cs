using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shipping;

public static class ShippingErrors
{
    public static readonly Error SellerIdRequired =
        new("ShippingProfile.Seller.Required", "SellerId is required.");

    public static readonly Error MethodTypeRequired =
        new("Shipping.Method.TypeRequired", "Shipping method type is required.");

    public static readonly Error CurrencyRequired =
        new("Shipping.Currency.Required", "Currency is required.");

    public static readonly Error BaseFeeCannotBeNegative =
        new("Shipping.BaseFee.Invalid", "Base fee cannot be negative.");

    public static readonly Error MethodAlreadyExists =
        new("ShippingProfile.Method.Exists", "Shipping method already exists.");

    public static readonly Error MethodNotFound =
        new("ShippingProfile.Method.NotFound", "Shipping method not found.");

    public static readonly Error EstimatedMinDaysInvalid =
        new("Shipping.EstimatedMinDays.Invalid", "Estimated min days cannot be negative.");

    public static readonly Error EstimatedMaxDaysInvalid =
        new("Shipping.EstimatedMaxDays.Invalid", "Estimated max days cannot be negative.");

    public static readonly Error EstimatedRangeInvalid =
        new("Shipping.EstimatedDays.RangeInvalid", "Estimated min days cannot exceed max days.");

    public static readonly Error FreeShippingAmountInvalid =
        new("Shipping.FreeShippingAmount.Invalid", "Free shipping threshold cannot be negative.");

    public static readonly Error ProfileMustHaveMethods =
        new("ShippingProfile.Empty", "Shipping profile must contain at least one method.");

    public static readonly Error ProfileMustHaveEnabledMethods =
        new("ShippingProfile.NoEnabledMethods", "At least one shipping method must be enabled.");

    public static readonly Error ArchivedProfileCannotBeModified =
        new("ShippingProfile.Archived.CannotModify", "Archived shipping profile cannot be modified.");
}