using BazaR.Backend.Domain.Common;
namespace BazaR.Backend.Domain.Orders.Events;

public sealed record OrderCancelledEvent(OrderId OrderId) : DomainEvent;
