using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Carts.Events;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Users;
using System;

public sealed class Cart : AggregateRoot<CartId>
{
    private const int MaxPerItem = 99;
    private readonly List<CartItem> _items = new();

    public IReadOnlyCollection<CartItem> Items => _items;

    public UserId UserId { get; private set; } = default!;
    public CartStatus Status { get; private set; } = CartStatus.Active;

    // фиксируем валюту корзины на первом добавлении
    public string Currency { get; private set; } = "UAH";

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? LastActivityAt { get; private set; }

    public int ItemsCount => _items.Count;
    public int TotalQuantity => _items.Sum(i => i.Quantity);

    public MoneySnapshot TotalPrice
    {
        get
        {
            var total = _items.Sum(i => i.PriceSnapshot.Amount * i.Quantity);
            return new MoneySnapshot(total, Currency);
        }
    }

    private Cart(UserId userId, DateTimeOffset nowUtc) : base(CartId.New())
    {
        UserId = userId;
        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;
        LastActivityAt = nowUtc;
    }

    private Cart() { } // EF

    public static Result<Cart> Create(UserId userId, DateTimeOffset? nowUtc = null)
    {
       
        if (userId.Value == Guid.Empty)
            return Result<Cart>.Failure(CartErrors.UserIdRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        var cart = new Cart(userId, now);
        cart.AddDomainEvent(new CartCreatedEvent(cart.Id, userId));
        return Result<Cart>.Success(cart);
    }

    private Result EnsureActive()
        => Status == CartStatus.Active
            ? Result.Success()
            : Result.Failure(CartErrors.CannotModifyNonActive);

    private void Touch(DateTimeOffset nowUtc)
    {
        UpdatedAt = nowUtc;
        LastActivityAt = nowUtc;
    }

    private Result EnsureCurrency(MoneySnapshot snapshot)
    {
        var itemCur = snapshot.Currency.Trim().ToUpperInvariant();

        if (_items.Count == 0)
        {
            Currency = itemCur;
            return Result.Success();
        }

        if (!string.Equals(Currency, itemCur, StringComparison.OrdinalIgnoreCase))
            return Result.Failure(CartErrors.CurrencyMismatch(Currency, itemCur));

        return Result.Success();
    }



    public Result AddItem(OfferId offerId, int quantity, MoneySnapshot priceSnapshot, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureActive();
        if (g.IsFailure) return g;

        if (offerId.Value == Guid.Empty)
            return Result.Failure(CartErrors.OfferRequired);

        if (quantity < 1)
            return Result.Failure(CartErrors.QuantityMustBePositive);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        var curGuard = EnsureCurrency(priceSnapshot);
        if (curGuard.IsFailure) return curGuard;

        var existing = _items.FirstOrDefault(i => i.OfferId == offerId);
        if (existing is not null)
        {
            var inc = existing.Increase(quantity, MaxPerItem, now);
            if (inc.IsFailure) return inc;

           
            existing.UpdatePriceSnapshot(priceSnapshot, now);
        }
        else
        {
            var itemRes = CartItem.Create(Id, offerId, quantity, priceSnapshot, now);
            if (itemRes.IsFailure) return Result.Failure(itemRes.Error);

            _items.Add(itemRes.Value!);
        }

        Touch(now);
        AddDomainEvent(new CartItemAddedEvent(Id, offerId, quantity, priceSnapshot));
        return Result.Success();
    }

    public Result UpdateItemQuantity(OfferId offerId, int newQuantity, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureActive();
        if (g.IsFailure) return g;

        if (offerId.Value == Guid.Empty)
            return Result.Failure(CartErrors.OfferRequired);

        var item = _items.FirstOrDefault(i => i.OfferId == offerId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        var res = item.SetQuantity(newQuantity, MaxPerItem, now);
        if (res.IsFailure) return res;

        Touch(now);
        AddDomainEvent(new CartItemQuantityChangedEvent(Id, offerId, newQuantity));
        return Result.Success();
    }

    public Result RemoveItem(OfferId offerId, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureActive();
        if (g.IsFailure) return g;

        if (offerId.Value == Guid.Empty)
            return Result.Failure(CartErrors.OfferRequired);

        var item = _items.FirstOrDefault(i => i.OfferId == offerId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        _items.Remove(item);

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        Touch(now);

        AddDomainEvent(new CartItemRemovedEvent(Id, offerId));
        return Result.Success();
    }

    public Result Clear(DateTimeOffset? nowUtc = null)
    {
        var g = EnsureActive();
        if (g.IsFailure) return g;

        if (_items.Count == 0)
            return Result.Success();

        _items.Clear();

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        Touch(now);

        AddDomainEvent(new CartClearedEvent(Id));
        return Result.Success();
    }

    public Result RefreshPriceSnapshot(OfferId offerId, MoneySnapshot newSnapshot, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureActive();
        if (g.IsFailure) return g;

        var item = _items.FirstOrDefault(i => i.OfferId == offerId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        var curGuard = EnsureCurrency(newSnapshot);
        if (curGuard.IsFailure) return curGuard;

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        item.UpdatePriceSnapshot(newSnapshot, now);

        Touch(now);
        AddDomainEvent(new CartPriceSnapshotUpdatedEvent(Id, offerId, newSnapshot));
        return Result.Success();
    }

    /// <summary>
    /// Корзина готова к оформлению: помечаем checked out.
    /// Сам Order создаётся в Application layer.
    /// </summary>
    public Result MarkCheckedOut(DateTimeOffset? nowUtc = null)
    {
        if (Status != CartStatus.Active)
            return Result.Success();

        if (_items.Count == 0)
            return Result.Failure(CartErrors.EmptyCart);

        Status = CartStatus.CheckedOut;

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        Touch(now);

        AddDomainEvent(new CartCheckedOutEvent(Id));
        return Result.Success();
    }

    public Result MarkAbandoned(DateTimeOffset? nowUtc = null)
    {
        if (Status == CartStatus.Abandoned)
            return Result.Success();

        if (Status == CartStatus.CheckedOut)
            return Result.Failure(new Error("Cart.CannotAbandonCheckedOut", "CheckedOut cart cannot be abandoned."));

        Status = CartStatus.Abandoned;

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        Touch(now);

        AddDomainEvent(new CartAbandonedEvent(Id));
        return Result.Success();
    }
}