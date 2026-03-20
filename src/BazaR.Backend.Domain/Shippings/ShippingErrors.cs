using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public static class ShippingErrors
{
    public static readonly Error ShippingIdRequired =
        new("Shipping.Id.Required", "Shipping id is required.");

    public static readonly Error OrderIdRequired =
        new("Shipping.OrderId.Required", "Order id is required.");

    public static readonly Error UserIdRequired =
        new("Shipping.UserId.Required", "User id is required.");

    public static readonly Error SellerIdRequired =
        new("Shipping.SellerId.Required", "Seller id is required.");

    public static readonly Error MethodTypeRequired =
        new("Shipping.MethodType.Required", "Shipping method type is required.");

    public static readonly Error RecipientRequired =
        new("Shipping.Recipient.Required", "Shipping recipient is required.");

    public static readonly Error DestinationRequired =
        new("Shipping.Destination.Required", "Shipping destination is required.");

    public static readonly Error CostRequired =
        new("Shipping.Cost.Required", "Shipping cost is required.");

    public static readonly Error RecipientFirstNameRequired =
        new("Shipping.Recipient.FirstName.Required", "Recipient first name is required.");

    public static readonly Error RecipientLastNameRequired =
        new("Shipping.Recipient.LastName.Required", "Recipient last name is required.");

    public static readonly Error RecipientPhoneRequired =
        new("Shipping.Recipient.Phone.Required", "Recipient phone is required.");

    public static readonly Error CountryRequired =
        new("Shipping.Destination.Country.Required", "Country is required.");

    public static readonly Error CityRequired =
        new("Shipping.Destination.City.Required", "City is required.");

    public static readonly Error FinalStatusCannotBeChanged =
        new("Shipping.Status.Final", "Final shipping status cannot be changed.");

    public static readonly Error OnlyPendingCanBePrepared =
        new("Shipping.Status.OnlyPendingCanBePrepared", "Only pending shipping can be moved to preparing.");

    public static readonly Error OnlyPreparingCanBeReadyToShip =
        new("Shipping.Status.OnlyPreparingCanBeReadyToShip", "Only preparing shipping can be moved to ready-to-ship.");

    public static readonly Error InvalidStatusForShip =
        new("Shipping.Status.InvalidForShip", "Shipping cannot be shipped from the current status.");

    public static readonly Error CarrierRequired =
        new("Shipping.Carrier.Required", "Carrier is required.");

    public static readonly Error TrackingNumberRequired =
        new("Shipping.TrackingNumber.Required", "Tracking number is required for non-pickup shipping.");

    public static readonly Error OnlyShippedCanBeDelivered =
        new("Shipping.Status.OnlyShippedCanBeDelivered", "Only shipped shipping can be marked as delivered.");

    public static readonly Error DeliveredCannotBeCancelled =
        new("Shipping.Status.DeliveredCannotBeCancelled", "Delivered shipping cannot be cancelled.");

    public static readonly Error ReturnedCannotBeCancelled =
        new("Shipping.Status.ReturnedCannotBeCancelled", "Returned shipping cannot be cancelled.");

    public static readonly Error OnlyShippedOrDeliveredCanBeReturned =
        new("Shipping.Status.OnlyShippedOrDeliveredCanBeReturned", "Only shipped or delivered shipping can be returned.");
}