using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders.Events;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _items = new();

    public UserId? BuyerUserId { get; private set; }
    public OrderCustomer Customer { get; private set; } = default!;
    public OrderDelivery Delivery { get; private set; } = default!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public string? CustomerComment { get; private set; }
    public string? CancellationReason { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public DateTimeOffset? PaidAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }
    public DateTimeOffset? DeliveredAtUtc { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order(
        OrderId id,
        UserId? buyerUserId,
        OrderCustomer customer,
        OrderDelivery delivery,
        string? customerComment) : base(id)
    {
        var now = DateTimeOffset.UtcNow;

        BuyerUserId = buyerUserId;
        Customer = customer;
        Delivery = delivery;
        Status = OrderStatus.Pending;
        CustomerComment = NormalizeOptional(customerComment);

        CreatedAtUtc = now;
        UpdatedAtUtc = now;
    }

    private Order() { }

    public static Result<Order> Create(
        UserId? buyerUserId,
        OrderCustomer customer,
        OrderDelivery delivery,
        IReadOnlyCollection<OrderLine> lines,
        string? customerComment = null)
    {
        if (lines is null || lines.Count == 0)
            return Result<Order>.Failure(OrderErrors.EmptyOrder);

        foreach (var line in lines)
        {
            if (line.Quantity <= 0)
                return Result<Order>.Failure(OrderErrors.InvalidQuantity);
        }

        var currency = lines.First().UnitPrice.Currency;
        if (lines.Any(l => l.UnitPrice.Currency != currency))
            return Result<Order>.Failure(OrderErrors.CurrencyMismatch);

        var order = new Order(OrderId.New(), buyerUserId, customer, delivery, customerComment);

        foreach (var line in lines)
        {
            var itemRes = OrderItem.Create(
                order.Id,
                line.ProductId,
                line.ProductName,
                line.Sku,
                line.Quantity,
                line.UnitPrice);

            if (itemRes.IsFailure)
                return Result<Order>.Failure(itemRes.Error);

            order._items.Add(itemRes.Value!);
        }

        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return Result<Order>.Success(order);
    }

    public Money Subtotal
    {
        get
        {
            var activeItems = _items.Where(x => x.ActiveQuantity > 0).ToList();
            if (activeItems.Count == 0)
                return Money.Create(0m, _items[0].PriceSnapshot.Currency).Value!;

            var currency = activeItems[0].PriceSnapshot.Currency;
            var sum = activeItems.Sum(i => i.PriceSnapshot.Amount * i.ActiveQuantity);
            return Money.Create(sum, currency).Value!;
        }
    }

    public Money DiscountTotal => Money.Create(0m, _items[0].PriceSnapshot.Currency).Value!;
    public Money DeliveryFee => Money.Create(0m, _items[0].PriceSnapshot.Currency).Value!;

    public Money GrandTotal
    {
        get
        {
            var currency = _items[0].PriceSnapshot.Currency;
            var total = Subtotal.Amount - DiscountTotal.Amount + DeliveryFee.Amount;
            return Money.Create(total, currency).Value!;
        }
    }

    public Result UpdateCustomerComment(string? comment)
    {
        if (Status is not OrderStatus.Pending and not OrderStatus.AwaitingPayment)
            return Result.Failure(OrderErrors.CommentChangeNotAllowed);

        CustomerComment = NormalizeOptional(comment);
        Touch();
        AddDomainEvent(new OrderCustomerCommentUpdatedEvent(Id));
        return Result.Success();
    }

    public Result MarkAwaitingPayment()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.OnlyPendingCanAwaitPayment);

        Status = OrderStatus.AwaitingPayment;
        Touch();
        AddDomainEvent(new OrderAwaitingPaymentEvent(Id));
        return Result.Success();
    }

    public Result Pay()
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.CannotPayCancelled);
        if (Status == OrderStatus.Paid)
            return Result.Failure(OrderErrors.AlreadyPaid);
        if (Status != OrderStatus.AwaitingPayment && Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.OnlyPendingOrAwaitingCanBePaid);

        Status = OrderStatus.Paid;
        PaidAtUtc = DateTimeOffset.UtcNow;
        Touch();
        AddDomainEvent(new OrderPaidEvent(Id));
        return Result.Success();
    }

    public Result StartProcessing()
    {
        if (Status != OrderStatus.Paid)
            return Result.Failure(OrderErrors.OnlyPaidCanBeProcessing);

        Status = OrderStatus.Processing;
        Touch();
        AddDomainEvent(new OrderProcessingStartedEvent(Id));
        return Result.Success();
    }

    public Result StartProcessingCashOnDelivery()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.OnlyPendingCanBeProcessing);

        Status = OrderStatus.Processing;
        Touch();
        AddDomainEvent(new OrderProcessingStartedEvent(Id));
        return Result.Success();
    }

    public Result Ship()
    {
        if (Status != OrderStatus.Processing)
            return Result.Failure(OrderErrors.OnlyProcessingCanBeShipped);

        Status = OrderStatus.Shipped;
        Touch();
        AddDomainEvent(new OrderShippedEvent(Id));
        return Result.Success();
    }

    public Result Deliver()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Failure(OrderErrors.OnlyShippedCanBeDelivered);

        Status = OrderStatus.Delivered;
        DeliveredAtUtc = DateTimeOffset.UtcNow;
        Touch();
        AddDomainEvent(new OrderDeliveredEvent(Id));
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status != OrderStatus.Delivered)
            return Result.Failure(OrderErrors.OnlyDeliveredCanBeCompleted);

        Status = OrderStatus.Completed;
        CompletedAtUtc = DateTimeOffset.UtcNow;
        Touch();
        AddDomainEvent(new OrderCompletedEvent(Id));
        return Result.Success();
    }

    public Result Cancel(string reason)
    {
        if (Status == OrderStatus.Paid)
            return Result.Failure(OrderErrors.CannotCancelPaid);
        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Completed)
            return Result.Failure(OrderErrors.CannotCancelAfterShipping);
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.AlreadyCancelled);
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(OrderErrors.CancellationReasonRequired);

        Status = OrderStatus.Cancelled;
        CancellationReason = reason.Trim();
        CancelledAtUtc = DateTimeOffset.UtcNow;
        Touch();
        AddDomainEvent(new OrderCancelledEvent(Id));
        return Result.Success();
    }

    public Result CancelItem(ProductId productId, int quantity, string reason)
    {
        if (Status is OrderStatus.Cancelled or OrderStatus.Delivered or OrderStatus.Completed)
            return Result.Failure(OrderErrors.PartialCancellationNotAllowed);
        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(OrderErrors.CancellationReasonRequired);

        var item = _items.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
            return Result.Failure(OrderErrors.ItemNotFound);

        var cancelRes = item.CancelQuantity(quantity);
        if (cancelRes.IsFailure)
            return cancelRes;

        Touch();
        AddDomainEvent(new OrderItemCancelledEvent(Id, productId, quantity, reason.Trim()));

        if (_items.All(x => x.ActiveQuantity == 0))
        {
            Status = OrderStatus.Cancelled;
            CancellationReason = "All order items were cancelled.";
            CancelledAtUtc = DateTimeOffset.UtcNow;
            AddDomainEvent(new OrderCancelledEvent(Id));
        }

        return Result.Success();
    }

    public Result ChangeDelivery(OrderDelivery newDelivery)
    {
        if (Status is not OrderStatus.Pending and not OrderStatus.AwaitingPayment)
            return Result.Failure(OrderErrors.DeliveryChangeNotAllowed);

        Delivery = newDelivery;
        Touch();
        AddDomainEvent(new OrderDeliveryChangedEvent(Id));
        return Result.Success();
    }

    private void Touch() => UpdatedAtUtc = DateTimeOffset.UtcNow;

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}