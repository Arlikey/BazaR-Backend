using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
namespace BazaR.Backend.Domain.Orders.Events;

public sealed record OrderCancelledEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderAwaitingPaymentEvent(OrderId OrderId) : DomainEvent;


public sealed record OrderProcessingStartedEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderShippedEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderDeliveredEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderCompletedEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderDeliveryAddressChangedEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderCustomerCommentUpdatedEvent(OrderId OrderId) : DomainEvent;

public sealed record OrderItemCancelledEvent(
    OrderId OrderId,
    ProductId ProductId,
    int Quantity,
    string Reason) : DomainEvent;


public sealed record OrderDeliveryChangedEvent(OrderId OrderId) : DomainEvent;