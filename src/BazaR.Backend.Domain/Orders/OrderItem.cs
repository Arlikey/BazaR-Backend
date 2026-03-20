using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Orders;

public sealed class OrderItem : Entity<long>
{
    public OrderId OrderId { get; private set; }
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string? Sku { get; private set; }

    public int Quantity { get; private set; }
    public int CancelledQuantity { get; private set; }
    public int ActiveQuantity => Quantity - CancelledQuantity;

    public Money PriceSnapshot { get; private set; } = default!;

    public Money LineTotal
        => Money.Create(PriceSnapshot.Amount * ActiveQuantity, PriceSnapshot.Currency).Value!;

    private OrderItem(
        OrderId orderId,
        ProductId productId,
        string productName,
        string? sku,
        int quantity,
        Money priceSnapshot)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Sku = sku;
        Quantity = quantity;
        CancelledQuantity = 0;
        PriceSnapshot = priceSnapshot;
    }

    private OrderItem() { }

    public static Result<OrderItem> Create(
        OrderId orderId,
        ProductId productId,
        string productName,
        string? sku,
        int quantity,
        Money priceSnapshot)
    {
        if (quantity <= 0)
            return Result<OrderItem>.Failure(OrderErrors.InvalidQuantity);

        if (string.IsNullOrWhiteSpace(productName))
            return Result<OrderItem>.Failure(OrderErrors.ProductNameRequired);

        return Result<OrderItem>.Success(new OrderItem(
            orderId,
            productId,
            productName.Trim(),
            string.IsNullOrWhiteSpace(sku) ? null : sku.Trim(),
            quantity,
            priceSnapshot
        ));
    }

    public Result CancelQuantity(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure(OrderErrors.InvalidCancellationQuantity);

        if (quantity > ActiveQuantity)
            return Result.Failure(OrderErrors.CancellationQuantityTooLarge);

        CancelledQuantity += quantity;
        return Result.Success();
    }
}