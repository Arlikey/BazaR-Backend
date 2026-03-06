using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users.Events;

namespace BazaR.Backend.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private readonly HashSet<UserRole> _roles = new();

    // ===== Profile =====
    public Email Email { get; private set; } = default!;
    public FullName Name { get; private set; } = default!;
    public PhoneNumber? Phone { get; private set; }

    // ===== Avatar (1 per user) =====
    private UserAvatar? _avatar;
    public UserAvatar? Avatar => _avatar;
    public string? GetAvatarUrl() => _avatar?.Url;

    // ===== Status =====
    public UserStatus Status { get; private set; } = UserStatus.Active;

    // ===== Audit =====
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles;

    private User(UserId id, Email email, FullName name, PhoneNumber? phone, DateTimeOffset nowUtc) : base(id)
    {
        Email = email;
        Name = name;
        Phone = phone;

        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;

        _roles.Add(UserRole.Customer);
    }

    private User() { } 

    public static Result<User> Create(
        Guid identityUserId,
        string email,
        string firstName,
        string lastName,
        string? phone = null,
        DateTimeOffset? nowUtc = null)
    {
        if (identityUserId == Guid.Empty)
            return Result<User>.Failure(UserErrors.IdentityIdRequired);

        var emailRes = Email.Create(email);
        if (emailRes.IsFailure)
            return Result<User>.Failure(emailRes.Error);

        var nameRes = FullName.Create(firstName, lastName);
        if (nameRes.IsFailure)
            return Result<User>.Failure(nameRes.Error);

        PhoneNumber? phoneVo = null;
        if (!string.IsNullOrWhiteSpace(phone))
        {
            var phoneRes = PhoneNumber.Create(phone);
            if (phoneRes.IsFailure)
                return Result<User>.Failure(phoneRes.Error);

            phoneVo = phoneRes.Value!;
        }

        var now = nowUtc ?? DateTimeOffset.UtcNow;
        var user = new User(new UserId(identityUserId), emailRes.Value!, nameRes.Value!, phoneVo, now);

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email.Value));
        user.AddDomainEvent(new UserProfileUpdatedEvent(user.Id));

        return Result<User>.Success(user);
    }

    private void Touch() => UpdatedAt = DateTimeOffset.UtcNow;

    private Result EnsureActive()
        => Status == UserStatus.Blocked
            ? Result.Failure(UserErrors.BlockedCannotBeModified)
            : Result.Success();

    // =========================================================
    // Profile
    // =========================================================

    public Result UpdateProfile(string firstName, string lastName, string? phone)
    {
        var guard = EnsureActive();
        if (guard.IsFailure) return guard;

        var nameRes = FullName.Create(firstName, lastName);
        if (nameRes.IsFailure)
            return Result.Failure(nameRes.Error);

        PhoneNumber? phoneVo = null;
        if (!string.IsNullOrWhiteSpace(phone))
        {
            var phoneRes = PhoneNumber.Create(phone);
            if (phoneRes.IsFailure)
                return Result.Failure(phoneRes.Error);

            phoneVo = phoneRes.Value!;
        }

        Name = nameRes.Value!;
        Phone = phoneVo;

        Touch();
        AddDomainEvent(new UserProfileUpdatedEvent(Id));
        return Result.Success();
    }

    public Result ChangeEmail(string newEmail)
    {
        var guard = EnsureActive();
        if (guard.IsFailure) return guard;

        var emailRes = Email.Create(newEmail);
        if (emailRes.IsFailure)
            return Result.Failure(emailRes.Error);

        if (Email.Equals(emailRes.Value))
            return Result.Success();

        Email = emailRes.Value!;
        Touch();
        AddDomainEvent(new UserEmailChangedEvent(Id, Email.Value));
        return Result.Success();
    }

    public Result ChangePhone(string? phone)
    {
        var guard = EnsureActive();
        if (guard.IsFailure) return guard;

        if (string.IsNullOrWhiteSpace(phone))
        {
            if (Phone is null) return Result.Success();
            Phone = null;
            Touch();
            AddDomainEvent(new UserPhoneChangedEvent(Id));
            return Result.Success();
        }

        var phoneRes = PhoneNumber.Create(phone);
        if (phoneRes.IsFailure)
            return Result.Failure(phoneRes.Error);

        if (Phone is not null && Phone.Equals(phoneRes.Value))
            return Result.Success();

        Phone = phoneRes.Value!;
        Touch();
        AddDomainEvent(new UserPhoneChangedEvent(Id));
        return Result.Success();
    }

    // =========================================================
    // Avatar
    // =========================================================

    public Result SetAvatar(string url, string storageKey, string contentType, long sizeBytes)
    {
        var guard = EnsureActive();
        if (guard.IsFailure) return guard;

        var now = DateTimeOffset.UtcNow;

        var avatarRes = UserAvatar.Create(
            userId: Id,
            url: url,
            storageKey: storageKey,
            contentType: contentType,
            sizeBytes: sizeBytes,
            nowUtc: now);

        if (avatarRes.IsFailure)
            return Result.Failure(avatarRes.Error);

        // idempotency
        if (_avatar is not null &&
            _avatar.Url == avatarRes.Value!.Url &&
            _avatar.StorageKey == avatarRes.Value!.StorageKey)
            return Result.Success();

        _avatar = avatarRes.Value!;

        Touch();
        AddDomainEvent(new UserAvatarChangedEvent(Id, _avatar.Url));
        return Result.Success();
    }

    public Result RemoveAvatar()
    {
        var guard = EnsureActive();
        if (guard.IsFailure) return guard;

        if (_avatar is null)
            return Result.Success();

        _avatar = null;

        Touch();
        AddDomainEvent(new UserAvatarRemovedEvent(Id));
        return Result.Success();
    }

    // =========================================================
    // Status
    // =========================================================

    public Result Block()
    {
        if (Status == UserStatus.Blocked)
            return Result.Failure(UserErrors.AlreadyBlocked);

        Status = UserStatus.Blocked;
        Touch();
        AddDomainEvent(new UserBlockedEvent(Id));
        return Result.Success();
    }

    public Result Unblock()
    {
        if (Status == UserStatus.Active)
            return Result.Failure(UserErrors.AlreadyActive);

        Status = UserStatus.Active;
        Touch();
        AddDomainEvent(new UserUnblockedEvent(Id));
        return Result.Success();
    }

    // =========================================================
    // Roles
    // =========================================================

    public bool HasRole(UserRole role) => _roles.Contains(role);

    public Result GrantRole(UserRole role)
    {
        if (_roles.Contains(role))
            return Result.Failure(UserErrors.RoleAlreadyGranted);

        _roles.Add(role);
        Touch();
        AddDomainEvent(new UserRoleGrantedEvent(Id, role));
        return Result.Success();
    }

    public Result RevokeRole(UserRole role)
    {
        if (!_roles.Contains(role))
            return Result.Failure(UserErrors.RoleNotAssigned);

        if (_roles.Count == 1)
            return Result.Failure(UserErrors.CannotRemoveLastRole);

        _roles.Remove(role);
        Touch();
        AddDomainEvent(new UserRoleRevokedEvent(Id, role));
        return Result.Success();
    }

    // =========================================================
    // Login marker
    // =========================================================

    public void MarkLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
        Touch();
        AddDomainEvent(new UserLoggedInEvent(Id, LastLoginAt.Value));
    }
}