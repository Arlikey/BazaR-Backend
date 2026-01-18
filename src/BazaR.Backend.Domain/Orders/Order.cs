using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders.Events;
using BazaR.Backend.Domain.Users;

using System.Collections.Generic;
using System.Linq;

namespace BazaR.Backend.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _items = new();

    public UserId UserId { get; private set; }
    public Address DeliveryAddress { get; private set; } = default!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order(OrderId id, UserId userId, Address address) : base(id)
    {
        UserId = userId;
        DeliveryAddress = address;
        Status = OrderStatus.Pending;
    }

    private Order() { } // for ORM

    // Factory Method (Named constructor) + invariants + domain event
    public static Result<Order> Create(UserId userId, Address address, IReadOnlyCollection<OrderLine> lines)
    {
        if (lines is null || lines.Count == 0)
            return Result<Order>.Failure(OrderErrors.EmptyOrder);

        // validate quantities & currency consistency
        foreach (var line in lines)
        {
            if (line.Quantity <= 0)
                return Result<Order>.Failure(OrderErrors.InvalidQuantity);
        }

        var currency = lines.First().PriceSnapshot.Currency;

        if (lines.Any(l => l.PriceSnapshot.Currency != currency))
            return Result<Order>.Failure(OrderErrors.CurrencyMismatch);

        var order = new Order(OrderId.New(), userId, address);

        foreach (var line in lines)
        {
            order._items.Add(new OrderItem(line.ProductId, line.Quantity, line.PriceSnapshot));
        }

        order.AddDomainEvent(new OrderCreatedEvent(order.Id));
        return Result<Order>.Success(order);
    }

    public Money Total
    {
        get
        {
            if (_items.Count == 0)
                return Money.Create(0.01m, "UAH").Value!; // никогда не должно случаться после инвариантов
                                                          // (можешь убрать и сделать отдельный safe-guard)
            var currency = _items[0].PriceSnapshot.Currency;
            var sum = _items.Sum(i => i.PriceSnapshot.Amount * i.Quantity);
            return Money.Create(sum, currency).Value!;
        }
    }

    // State machine: Pending -> Paid
    public Result Pay()
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.CannotPayCancelled);

        if (Status == OrderStatus.Paid)
            return Result.Failure(OrderErrors.AlreadyPaid);

        Status = OrderStatus.Paid;
        AddDomainEvent(new OrderPaidEvent(Id));
        return Result.Success();
    }

    // State machine: Pending -> Cancelled
    public Result Cancel()
    {
        if (Status == OrderStatus.Paid)
            return Result.Failure(OrderErrors.CannotCancelPaid);

        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.AlreadyCancelled);

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledEvent(Id));
        return Result.Success();
    }

    // Optional behavior: user can update address only while Pending
    public Result ChangeDeliveryAddress(Address newAddress)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(new Error("order.address.change.notAllowed", "Cannot change address after payment/cancellation.", ErrorType.Conflict));

        DeliveryAddress = newAddress;
        return Result.Success();
    }
}
