using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;

namespace BazaR.Backend.Domain.Payments;

public sealed record PaymentCreatedDomainEvent(PaymentId PaymentId, OrderId OrderId) : DomainEvent;
public sealed record PaymentCheckoutAttachedDomainEvent(PaymentId PaymentId, OrderId OrderId) : DomainEvent;
public sealed record PaymentAuthorizedDomainEvent(PaymentId PaymentId, OrderId OrderId) : DomainEvent;
public sealed record PaymentPaidDomainEvent(PaymentId PaymentId, OrderId OrderId) : DomainEvent;
public sealed record PaymentFailedDomainEvent(PaymentId PaymentId, OrderId OrderId, string? Code, string? Message) : DomainEvent;
public sealed record PaymentCancelledDomainEvent(PaymentId PaymentId, OrderId OrderId) : DomainEvent;
public sealed record PaymentRefundedDomainEvent(PaymentId PaymentId, OrderId OrderId, decimal Amount, decimal TotalRefunded) : DomainEvent;