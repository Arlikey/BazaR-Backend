namespace BazaR.Backend.Api.Contracts.Carts;
public sealed record AddCartItemRequest(Guid OfferId, int Quantity);

public sealed record UpdateCartItemQuantityRequest(int Quantity);