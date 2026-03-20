using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Checkouts;

public sealed record CheckoutSubmittedDomainEvent(CheckoutId CheckoutId) : DomainEvent;