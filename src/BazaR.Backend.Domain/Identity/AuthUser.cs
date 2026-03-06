using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Identity.Events;

namespace BazaR.Backend.Domain.Identity;

public sealed class AuthUser : AggregateRoot<Guid>
{
    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public Guid UserId { get; private set; }                 // ссылка на бизнес User
    public IdentityEmail Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public bool IsBlocked { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private AuthUser() { }

    private AuthUser(Guid id, Guid userId, IdentityEmail email, string passwordHash)
        : base(id)
    {
        UserId = userId;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTimeOffset.UtcNow;
        IsBlocked = false;
    }

    public static Result<AuthUser> Register(Guid userId, string email, string passwordHash)
    {
        if (userId == Guid.Empty)
            return Result<AuthUser>.Failure(new Error("Identity.UserIdRequired", "UserId is required."));

        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result<AuthUser>.Failure(new Error("Identity.PasswordHashRequired", "Password hash is required."));

        var emailRes = IdentityEmail.Create(email);
        if (emailRes.IsFailure) return Result<AuthUser>.Failure(emailRes.Error);

        var au = new AuthUser(Guid.NewGuid(), userId, emailRes.Value!, passwordHash.Trim());
        au.AddDomainEvent(new AuthUserRegisteredEvent(au.Id, au.UserId, au.Email.Value));
        return Result<AuthUser>.Success(au);
    }

    public Result SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            return Result.Failure(new Error("Identity.PasswordHashRequired", "Password hash is required."));
        PasswordHash = passwordHash.Trim();
        AddDomainEvent(new AuthUserPasswordChangedEvent(Id));
        return Result.Success();
    }

    public Result Block()
    {
        if (IsBlocked) return Result.Success();
        IsBlocked = true;
        AddDomainEvent(new AuthUserBlockedEvent(Id));
        return Result.Success();
    }

    public Result Unblock()
    {
        if (!IsBlocked) return Result.Success();
        IsBlocked = false;
        AddDomainEvent(new AuthUserUnblockedEvent(Id));
        return Result.Success();
    }

    public RefreshToken IssueRefreshToken(string refreshTokenHash, TimeSpan ttl)
    {
        var rt = RefreshToken.Create(refreshTokenHash, ttl);
        _refreshTokens.Add(rt);
        AddDomainEvent(new RefreshTokenIssuedEvent(Id, rt.Id));
        return rt;
    }

    public Result RevokeRefreshToken(Guid refreshTokenId)
    {
        var rt = _refreshTokens.SingleOrDefault(x => x.Id == refreshTokenId);
        if (rt is null) return Result.Failure(new Error("Identity.RefreshNotFound", "Refresh token not found."));
        rt.Revoke();
        AddDomainEvent(new RefreshTokenRevokedEvent(Id, rt.Id));
        return Result.Success();
    }

    public RefreshToken? FindActiveRefreshByHash(string refreshTokenHash)
        => _refreshTokens.FirstOrDefault(x => x.IsActive && x.TokenHash == refreshTokenHash);
}
