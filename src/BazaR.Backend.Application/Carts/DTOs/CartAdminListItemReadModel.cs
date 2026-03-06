
namespace BazaR.Backend.Application.Carts.DTOs;

public sealed class CartAdminListItemReadModel
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
}