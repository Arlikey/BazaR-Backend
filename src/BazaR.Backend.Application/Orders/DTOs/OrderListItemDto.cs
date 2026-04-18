namespace BazaR.Backend.Application.Orders.DTOs;

public sealed record OrderListItemDto(
    Guid Id,
    string Number,
    int Status,
    string StatusText,
    decimal TotalAmount,
    string Currency,
    int ItemsCount,
    string? PreviewImageUrl,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DeliveredAtUtc,
    DateTimeOffset? CompletedAtUtc
);