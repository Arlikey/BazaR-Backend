using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;

public sealed class CartItem : Entity<Guid>
{
    public CartId CartId { get; private set; } = default!;
    public OfferId OfferId { get; private set; } = default!;
    public int Quantity { get; private set; }

    public MoneySnapshot PriceSnapshot { get; private set; } = default!;

    public DateTimeOffset AddedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private CartItem(
        Guid id,
        CartId cartId,
        OfferId offerId,
        int quantity,
        MoneySnapshot priceSnapshot,
        DateTimeOffset nowUtc)
        : base(id)
    {
        CartId = cartId;
        OfferId = offerId;
        Quantity = quantity;
        PriceSnapshot = priceSnapshot;

        AddedAt = nowUtc;
        UpdatedAt = nowUtc;
    }

    private CartItem() { } 

    internal static Result<CartItem> Create(
        CartId cartId,
        OfferId offerId,
        int quantity,
        MoneySnapshot priceSnapshot,
        DateTimeOffset nowUtc)
    {
        if (cartId.Value == Guid.Empty)
            return Result<CartItem>.Failure(new Error("CartItem.CartRequired", "Cart is required."));

        if (offerId.Value == Guid.Empty)
            return Result<CartItem>.Failure(CartErrors.OfferRequired);

        if (quantity < 1)
            return Result<CartItem>.Failure(CartErrors.QuantityMustBePositive);

        return Result<CartItem>.Success(
            new CartItem(Guid.NewGuid(), cartId, offerId, quantity, priceSnapshot, nowUtc));
    }

    internal Result Increase(int delta, int maxPerItem, DateTimeOffset nowUtc)
    {
        if (delta < 1)
            return Result.Failure(CartErrors.QuantityMustBePositive);

        var newQty = Quantity + delta;
        if (newQty > maxPerItem)
            return Result.Failure(CartErrors.MaxQuantityExceeded);

        Quantity = newQty;
        UpdatedAt = nowUtc;
        return Result.Success();
    }

    internal Result SetQuantity(int newQuantity, int maxPerItem, DateTimeOffset nowUtc)
    {
        if (newQuantity < 1)
            return Result.Failure(CartErrors.QuantityMustBePositive);

        if (newQuantity > maxPerItem)
            return Result.Failure(CartErrors.MaxQuantityExceeded);

        if (Quantity == newQuantity)
            return Result.Success();

        Quantity = newQuantity;
        UpdatedAt = nowUtc;
        return Result.Success();
    }

    internal void UpdatePriceSnapshot(MoneySnapshot snapshot, DateTimeOffset nowUtc)
    {
        PriceSnapshot = snapshot;
        UpdatedAt = nowUtc;
    }
}