using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users.Events;

using System.Collections.Generic;
using System.Linq;

namespace BazaR.Backend.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private readonly HashSet<UserRole> _roles = new();

    public Email Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public UserStatus Status { get; private set; } = UserStatus.Active;

    public IReadOnlyCollection<UserRole> Roles => _roles.ToList().AsReadOnly();

    private User(UserId id, Email email, string passwordHash) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;

      
        _roles.Add(UserRole.Customer);
    }

    private User() { } 

    
    public static Result<User> Register(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<User>.Failure(UserErrors.PasswordHashRequired);

        var emailRes = Email.Create(email);
        if (emailRes.IsFailure)
            return Result<User>.Failure(emailRes.Error);

        var user = new User(UserId.New(), emailRes.Value!, passwordHash.Trim());

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email.Value));

        return Result<User>.Success(user);
    }

    public Result Block()
    {
        if (Status == UserStatus.Blocked)
            return Result.Failure(UserErrors.AlreadyBlocked);

        Status = UserStatus.Blocked;
        AddDomainEvent(new UserBlockedEvent(Id));
        return Result.Success();
    }

    public Result Unblock()
    {
        if (Status == UserStatus.Active)
            return Result.Failure(UserErrors.AlreadyActive);

        Status = UserStatus.Active;
        return Result.Success();
    }

    public Result GrantRole(UserRole role)
    {
        if (_roles.Contains(role))
            return Result.Failure(UserErrors.RoleAlreadyGranted);

        _roles.Add(role);
        return Result.Success();
    }

    public Result RevokeRole(UserRole role)
    {
        if (!_roles.Contains(role))
            return Result.Failure(UserErrors.RoleNotAssigned);

        if (_roles.Count == 1)
            return Result.Failure(UserErrors.CannotRemoveLastRole);

        _roles.Remove(role);
        return Result.Success();
    }

    public bool HasRole(UserRole role) => _roles.Contains(role);

    public Result ChangeEmail(string newEmail)
    {
        var emailRes = Email.Create(newEmail);
        if (emailRes.IsFailure)
            return Result.Failure(emailRes.Error);

        Email = emailRes.Value!;
        return Result.Success();
    }
    public Result SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure(UserErrors.PasswordHashRequired);

        PasswordHash = passwordHash.Trim();
        return Result.Success();
    }
}
