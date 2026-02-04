using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sales.Events;

public sealed record OfferArchivedEvent(OfferId OfferId) : DomainEvent;
