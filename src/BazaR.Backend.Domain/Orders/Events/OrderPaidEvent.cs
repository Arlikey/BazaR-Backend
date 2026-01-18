using BazaR.Backend.Domain.Common;


namespace BazaR.Backend.Domain.Orders.Events;

public sealed record OrderPaidEvent(OrderId OrderId) : DomainEvent;
