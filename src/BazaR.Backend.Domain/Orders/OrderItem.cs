using System;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public sealed class OrderItem : Entity<Guid>
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money PriceSnapshot { get; private set; } = default!;

    internal OrderItem(ProductId productId, int quantity, Money priceSnapshot)
        : base(Guid.NewGuid())
    {
        ProductId = productId;
        Quantity = quantity;
        PriceSnapshot = priceSnapshot;
    }

    private OrderItem() { }
}
