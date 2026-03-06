namespace BazaR.Backend.Domain.Carts.Events;

using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Users;

public sealed record CartCreatedEvent(CartId CartId, UserId UserId) : DomainEvent;

public sealed record CartItemAddedEvent(CartId CartId, OfferId OfferId, int Quantity, MoneySnapshot PriceSnapshot) : DomainEvent;

public sealed record CartItemRemovedEvent(CartId CartId, OfferId OfferId) : DomainEvent;

public sealed record CartItemQuantityChangedEvent(CartId CartId, OfferId OfferId, int Quantity) : DomainEvent;

public sealed record CartClearedEvent(CartId CartId) : DomainEvent;

public sealed record CartCheckedOutEvent(CartId CartId) : DomainEvent;

public sealed record CartAbandonedEvent(CartId CartId) : DomainEvent;
    
public sealed record CartPriceSnapshotUpdatedEvent(CartId CartId, OfferId OfferId, MoneySnapshot PriceSnapshot) : DomainEvent;