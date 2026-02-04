using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sales.Events;

public sealed record OfferActivatedEvent(
    OfferId OfferId
) : DomainEvent;
