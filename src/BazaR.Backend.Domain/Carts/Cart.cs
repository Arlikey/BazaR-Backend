using BazaR.Backend.Domain.Carts.Events;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;

using System.Collections.Generic;
using System.Linq;

namespace BazaR.Backend.Domain.Carts;

public sealed class Cart : AggregateRoot<CartId>
{
    private readonly List<CartItem> _items = new();

    public UserId UserId { get; private set; }
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart(CartId id, UserId userId) : base(id)
    {
        UserId = userId;
    }

    private Cart() { } 

    
    public static Cart Create(UserId userId)
        => new(CartId.New(), userId);

    public bool IsEmpty => _items.Count == 0;

    public Money? Total
        => _items.Count == 0
            ? null
            : Money.Create(_items.Sum(i => i.PriceSnapshot.Amount * i.Quantity), _items[0].PriceSnapshot.Currency).Value;

    public Result AddItem(ProductId productId, int quantity, Money priceSnapshot)
    {
        if (quantity <= 0)
            return Result.Failure(CartErrors.InvalidQuantity);

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existing is null)
        {
            _items.Add(new CartItem(productId, quantity, priceSnapshot));
        }
        else
        {
            existing.Increase(quantity);
            existing.UpdatePriceSnapshot(priceSnapshot);
        }

        AddDomainEvent(new CartItemAddedEvent(Id, productId, quantity));
        return Result.Success();
    }

    public Result SetItemQuantity(ProductId productId, int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(CartErrors.InvalidQuantity);

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        item.SetQuantity(quantity);
        return Result.Success();
    }

    public Result RemoveItem(ProductId productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        _items.Remove(item);
        return Result.Success();
    }

    public Result Clear()
    {
        if (_items.Count == 0)
            return Result.Failure(CartErrors.EmptyCart);

        _items.Clear();
        AddDomainEvent(new CartClearedEvent(Id));
        return Result.Success();
    }
    public Result UpdatePriceSnapshot(ProductId productId, Money priceSnapshot)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure(CartErrors.ItemNotFound);

        item.UpdatePriceSnapshot(priceSnapshot);
        return Result.Success();
    }
}