using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Carts;

public sealed class CartItem : Entity<Guid>
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money PriceSnapshot { get; private set; } = default!;

    internal CartItem(ProductId productId, int quantity, Money priceSnapshot)
        : base(Guid.NewGuid())
    {
        ProductId = productId;
        Quantity = quantity;
        PriceSnapshot = priceSnapshot;
    }

    private CartItem() { } 

    internal void Increase(int value)
    {
        Quantity += value;
    }

    internal void SetQuantity(int value)
    {
        Quantity = value;
    }

    internal void UpdatePriceSnapshot(Money priceSnapshot)
    {
        PriceSnapshot = priceSnapshot;
    }
}
