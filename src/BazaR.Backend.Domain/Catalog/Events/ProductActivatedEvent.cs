using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Events;

public sealed record ProductActivatedEvent(ProductId ProductId) : DomainEvent;
