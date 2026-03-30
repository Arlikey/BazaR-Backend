namespace BazaR.Backend.Api.Contracts.Seller.Shippings;

public sealed record CreateNovaPoshtaShipmentRequest(
    string SenderCityRef,
    string SenderRef,
    string SenderAddressRef,
    string ContactSenderRef,
    string SendersPhone,
    string RecipientCityRef,
    string RecipientRef,
    string RecipientAddressRef,
    string ContactRecipientRef,
    string RecipientsPhone,
    decimal WeightKg,
    decimal DeclaredValue,
    decimal? CashOnDeliveryAmount,
    string Description);