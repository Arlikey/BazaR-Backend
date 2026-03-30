namespace BazaR.Backend.Api.Contracts.Seller.Shippings;

public sealed record NovaPoshtaTrackingSyncResponse(
    string? TrackingNumber,
    string? RawStatusCode,
    string? RawStatusName,
    bool IsCreated,
    bool IsInTransit,
    bool IsArrivedAtPickupPoint,
    bool IsDelivered);