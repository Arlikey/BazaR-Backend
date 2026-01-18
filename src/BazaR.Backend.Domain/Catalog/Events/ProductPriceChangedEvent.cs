using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Events;

public sealed record ProductPriceChangedEvent(ProductId ProductId, decimal NewAmount, string Currency) : DomainEvent;
