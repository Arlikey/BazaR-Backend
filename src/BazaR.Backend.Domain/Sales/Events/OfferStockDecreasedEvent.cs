using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sales.Events;

public sealed record OfferStockDecreasedEvent(
    OfferId OfferId,
    int Amount,
    int NewStock
) : DomainEvent;
