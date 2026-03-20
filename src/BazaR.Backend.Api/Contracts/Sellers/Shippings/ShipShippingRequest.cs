namespace BazaR.Backend.Api.Contracts.Seller.Shippings;

public sealed record ShipShippingRequest(
    string Carrier,
    string TrackingNumber,
    string? TrackingUrl,
    string? ExternalShipmentId);