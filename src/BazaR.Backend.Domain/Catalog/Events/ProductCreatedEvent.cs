using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Events;

public sealed record ProductCreatedEvent(ProductId ProductId) : DomainEvent;
