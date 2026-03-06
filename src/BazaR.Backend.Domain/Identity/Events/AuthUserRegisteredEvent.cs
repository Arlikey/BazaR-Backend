using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Identity.Events;

public sealed record AuthUserRegisteredEvent(Guid AuthUserId, Guid UserId, string Email) : DomainEvent;
public sealed record AuthUserPasswordChangedEvent(Guid AuthUserId) : DomainEvent;
public sealed record AuthUserBlockedEvent(Guid AuthUserId) : DomainEvent;
public sealed record AuthUserUnblockedEvent(Guid AuthUserId) : DomainEvent;

public sealed record RefreshTokenIssuedEvent(Guid AuthUserId, Guid RefreshTokenId) : DomainEvent;
public sealed record RefreshTokenRevokedEvent(Guid AuthUserId, Guid RefreshTokenId) : DomainEvent;
