namespace BazaR.Backend.Application.Shippings.DTOs;

public sealed record ShippingDetailsDto(
    Guid Id,
    Guid OrderId,
    Guid SellerId,
    Guid CustomerId,
    int Method,
    int SettlementMode,
    int Status,

    string RecipientFirstName,
    string RecipientLastName,
    string RecipientFullName,
    string RecipientPhone,
    string? RecipientEmail,

    string Country,
    string Region,
    string City,
    string? Street,
    string? House,
    string? Apartment,
    string? PostalCode,
    string? PickupPointCode,
    string? PickupPointName,

    string? SenderName,
    string? SenderPhone,
    string? SenderCountryCode,
    string? SenderPickupPointCode,
    string? SenderPickupPointName,

    string? TrackingNumber,
    IReadOnlyCollection<ShippingParcelDto> Parcels,

    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? DispatchedAtUtc,
    DateTimeOffset? ReadyForPickupAtUtc,
    DateTimeOffset? DeliveredAtUtc,
    DateTimeOffset? CancelledAtUtc);