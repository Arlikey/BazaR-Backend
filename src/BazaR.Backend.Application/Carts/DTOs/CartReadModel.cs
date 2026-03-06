
namespace BazaR.Backend.Application.Carts.DTOs;
public sealed class CartReadModel
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Currency { get; init; } = "UAH";

    public int ItemsCount { get; init; }
    public int TotalQuantity { get; init; }
    public decimal TotalAmount { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }

    public IReadOnlyList<CartItemReadModel> Items { get; init; } = Array.Empty<CartItemReadModel>();
}

public sealed class CartItemReadModel
{
    public Guid OfferId { get; init; }
    public int Quantity { get; init; }
    public decimal PriceAmount { get; init; }
    public string Currency { get; init; } = "UAH";
    public DateTimeOffset UpdatedAt { get; init; }
}


//Customer
public record CustomerCartItemDto(
    Guid OfferId,
    string ProductName,
    string? ProductSlug,
    string? MainImageUrl,
    string? Description,
    string SellerName,
    int Quantity,
    decimal PriceAmount,
    string Currency,
    decimal TotalPrice
);

public record CustomerCartDto(
    Guid Id,
    string Status,
    string Currency,
    int ItemsCount,
    int TotalQuantity,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    List<CustomerCartItemDto> Items
);