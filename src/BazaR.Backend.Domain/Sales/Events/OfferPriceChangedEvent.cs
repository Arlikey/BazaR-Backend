using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sales.Events;

public sealed record OfferPriceChangedEvent(
    OfferId OfferId,
    decimal Amount,
    string Currency
) : DomainEvent;
