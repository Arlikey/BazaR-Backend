using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Shippings;

public static class ShippingErrors
{
    public static readonly Error ShippingIdRequired =
        new("Shipping.Id.Required", "Shipping id is required.");

    public static readonly Error OrderIdRequired =
        new("Shipping.OrderId.Required", "Order id is required.");

    public static readonly Error CustomerIdRequired =
        new("Shipping.CustomerId.Required", "Customer id is required.");

    public static readonly Error SellerIdRequired =
        new("Shipping.SellerId.Required", "Seller id is required.");

    public static readonly Error MethodRequired =
        new("Shipping.Method.Required", "Shipping method is required.");

    public static readonly Error SettlementModeRequired =
        new("Shipping.SettlementMode.Required", "Shipping settlement mode is required.");

    public static readonly Error RecipientRequired =
        new("Shipping.Recipient.Required", "Shipping recipient is required.");

    public static readonly Error DestinationRequired =
        new("Shipping.Destination.Required", "Shipping destination is required.");

    public static readonly Error CountryRequired =
        new("Shipping.Country.Required", "Country is required.");

    public static readonly Error CityRequired =
        new("Shipping.City.Required", "City is required.");

    public static readonly Error RecipientFirstNameRequired =
        new("Shipping.Recipient.FirstName.Required", "Recipient first name is required.");

    public static readonly Error RecipientLastNameRequired =
        new("Shipping.Recipient.LastName.Required", "Recipient last name is required.");

    public static readonly Error RecipientPhoneRequired =
        new("Shipping.Recipient.Phone.Required", "Recipient phone is required.");

    public static readonly Error SenderRequired =
        new("Shipping.Sender.Required", "Shipping sender is required.");

    public static readonly Error ParcelRequired =
        new("Shipping.Parcel.Required", "Shipping parcel is required.");

    public static readonly Error ParcelsRequired =
        new("Shipping.Parcels.Required", "At least one parcel is required.");

    public static readonly Error TrackingNumberRequired =
        new("Shipping.TrackingNumber.Required", "Tracking number is required.");

    public static readonly Error FinalStatusCannotBeChanged =
        new("Shipping.Status.Final", "Final shipping status cannot be changed.");

    public static readonly Error DeliveredCannotBeCancelled =
        new("Shipping.Cancel.Delivered", "Delivered shipping cannot be cancelled.");

    public static readonly Error OnlyReadyToDispatchCanBeDispatched =
        new("Shipping.Dispatch.InvalidStatus", "Only ready to dispatch shipping can be dispatched.");

    public static readonly Error OnlyDispatchedCanBeReadyForPickup =
        new("Shipping.ReadyForPickup.InvalidStatus", "Only dispatched shipping can be marked as ready for pickup.");

    public static readonly Error OnlyReadyForPickupOrDispatchedCanBeDelivered =
        new("Shipping.Delivered.InvalidStatus", "Only ready for pickup or dispatched shipping can be delivered.");

    public static readonly Error SenderDivisionRequiredForNovaPoshta =
        new("Shipping.Sender.NovaPoshtaDivision.Required", "Sender Nova Poshta division is required.");

    public static readonly Error RecipientDivisionRequiredForNovaPoshta =
        new("Shipping.Recipient.NovaPoshtaDivision.Required", "Recipient Nova Poshta division is required.");
}