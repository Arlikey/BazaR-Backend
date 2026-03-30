namespace BazaR.Backend.Api.Contracts.Seller.Shippings;

public sealed record NovaPoshtaShipmentResponse(
    string? TrackingNumber,
    string? ExternalShipmentId,
    string? TrackingUrl,
    string? RawStatusCode,
    string? RawStatusName);