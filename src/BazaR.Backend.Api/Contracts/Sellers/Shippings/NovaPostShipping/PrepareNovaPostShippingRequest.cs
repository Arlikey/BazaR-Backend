using BazaR.Backend.Domain.Shippings;

namespace BazaR.Backend.Api.Contracts.Sellers.Shippings.NovaPostShipping
{


    public sealed record PrepareNovaPostShippingRequest(
    string? SenderName,
    string? SenderPhone,
    string? SenderEmail,
    string? SenderCountryCode,
    string? SenderDivisionId,
    string? SenderDivisionName,
    ShippingSettlementMode SettlementMode,
    decimal? CashOnDeliveryAmount,
    string? Comment,
    IReadOnlyCollection<PrepareNovaPostShippingParcelItemRequest> Parcels);
}
