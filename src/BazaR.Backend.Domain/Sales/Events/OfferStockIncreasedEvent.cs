using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sales.Events;

public sealed record OfferStockIncreasedEvent(
    OfferId OfferId,
    int Amount,
    int NewStock
) : DomainEvent;
