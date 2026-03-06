using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Users.Events;

// Создание доменного User (обычно после Identity.Create)
public sealed record UserCreatedEvent(UserId UserId, string Email) : DomainEvent;

// Если ты хочешь событие именно "Register" (может совпадать с Created — на твой выбор)


// Блокировка/разблокировка
public sealed record UserBlockedEvent(UserId UserId) : DomainEvent;
public sealed record UserUnblockedEvent(UserId UserId) : DomainEvent;

// Роли
public sealed record UserRoleGrantedEvent(UserId UserId, UserRole Role) : DomainEvent;
public sealed record UserRoleRevokedEvent(UserId UserId, UserRole Role) : DomainEvent;

// Email
public sealed record UserEmailChangedEvent(UserId UserId, string NewEmail) : DomainEvent;


public sealed record UserProfileUpdatedEvent(UserId UserId) : DomainEvent;
public sealed record UserPhoneChangedEvent(UserId UserId) : DomainEvent;
public sealed record UserLoggedInEvent(UserId UserId, DateTimeOffset At) : DomainEvent;


public sealed record UserAvatarChangedEvent(UserId UserId, string Url) : DomainEvent;

public sealed record UserAvatarRemovedEvent(UserId UserId) : DomainEvent;