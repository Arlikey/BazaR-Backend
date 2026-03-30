namespace BazaR.Backend.Application.Shippings.DTOs;

public sealed record ShippingListItemDto(
    Guid Id,
    Guid OrderId,
    Guid SellerId,
    Guid CustomerId,
    int Method,
    int SettlementMode,
    int Status,
    string RecipientFullName,
    string RecipientPhone,
    string City,
    string? PickupPointName,
    string? TrackingNumber,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DispatchedAtUtc,
    DateTimeOffset? DeliveredAtUtc);