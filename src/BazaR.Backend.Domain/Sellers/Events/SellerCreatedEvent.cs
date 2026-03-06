using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;

namespace BazaR.Backend.Domain.Sellers.Events;

public sealed record SellerCreatedEvent(SellerId SellerId) 
    : DomainEvent;
public sealed record SellerSubmittedForApprovalEvent(SellerId SellerId) : DomainEvent;
public sealed record SellerApprovedEvent(
    SellerId SellerId,
    Guid OwnerUserId,
    Guid ApprovedBy,
    DateTimeOffset ApprovedAt
) : DomainEvent;

public sealed record SellerRejectedEvent(SellerId SellerId, Guid AdminUserId, string Reason) : DomainEvent;
public sealed record SellerSuspendedEvent(SellerId SellerId, string? Reason) : DomainEvent;
public sealed record SellerClosedEvent(SellerId SellerId) : DomainEvent;
public sealed record SellerSlugChangedEvent(SellerId SellerId, string NewSlug) : DomainEvent;

public sealed record SellerReactivatedEvent(SellerId SellerId) : DomainEvent
{


};

