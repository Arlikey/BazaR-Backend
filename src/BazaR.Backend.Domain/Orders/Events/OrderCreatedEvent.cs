using BazaR.Backend.Domain.Common;


namespace BazaR.Backend.Domain.Orders.Events;

public sealed record OrderCreatedEvent(OrderId OrderId) : DomainEvent;
