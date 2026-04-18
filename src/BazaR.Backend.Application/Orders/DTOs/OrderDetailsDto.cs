namespace BazaR.Backend.Application.Orders.DTOs;

public sealed record OrderDetailsDto(
    Guid Id,
    string Number,
    int Status,
    string StatusText,
    decimal SubtotalAmount,
    decimal DeliveryFeeAmount,
    decimal DiscountAmount,
    decimal GrandTotalAmount,
    string Currency,

    string CustomerFirstName,
    string CustomerLastName,
    string CustomerPhone,
    string CustomerEmail,

    string DeliveryMethod,
    string City,
    string? Region,
    string? Warehouse,
    string? Street,
    string? Building,
    string? Apartment,
    string? PostalCode,

    string? CustomerComment,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? PaidAtUtc,
    DateTimeOffset? DeliveredAtUtc,
    DateTimeOffset? CompletedAtUtc,

    IReadOnlyCollection<OrderItemDto> Items
);