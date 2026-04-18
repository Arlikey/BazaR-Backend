namespace BazaR.Backend.Application.Orders.DTOs;

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string? Sku,
    int Quantity,
    decimal UnitPrice,
    string Currency,
    string? ImageUrl
);