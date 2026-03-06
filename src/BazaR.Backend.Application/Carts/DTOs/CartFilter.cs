namespace BazaR.Backend.Application.Carts.DTOs;
public sealed class CartFilter
{
    public Guid? UserId { get; init; }
    public string? Status { get; init; }
}