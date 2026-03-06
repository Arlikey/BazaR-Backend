using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions.Security;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Identity;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Identity.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IAuthUserRepository _authUsers;
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtProvider _jwt;
    private readonly IUnitOfWork _uow;

    // TTL можно вынести в options
    private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(30);

    public LoginCommandHandler(
        IAuthUserRepository authUsers,
        IUserRepository users,
        IPasswordHasher hasher,
        IJwtProvider jwt,
        IUnitOfWork uow)
    {
        _authUsers = authUsers;
        _users = users;
        _hasher = hasher;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        var emailRes = IdentityEmail.Create(request.Email);
        if (emailRes.IsFailure)
            return Result<AuthResponse>.Failure(emailRes.Error);

        var auth = await _authUsers.GetByEmailAsync(emailRes.Value!.Value, ct); 
        if (auth is null)
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));

        if (auth.IsBlocked)
            return Result<AuthResponse>.Failure(new Error("Auth.Blocked", "User is blocked."));

        if (!_hasher.Verify(request.Password, auth.PasswordHash))
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidCredentials", "Invalid email or password."));

        // роли берём из бизнес User
        var user = await _users.GetByIdAsync(new UserId(auth.UserId), ct);
        if (user is null)
            return Result<AuthResponse>.Failure(new Error("User.NotFound", "User not found."));

        var roles = user.Roles.Select(r => r.ToString()).ToList();

        var (access, accessExp) = _jwt.CreateAccessToken(user.Id.Value, roles);

        var refreshRaw = _jwt.CreateRefreshToken();
        var refreshHash = _jwt.HashRefreshToken(refreshRaw);
        var refresh = auth.IssueRefreshToken(refreshHash, RefreshTtl);

        _authUsers.Update(auth);
        await _uow.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(new AuthResponse(
            UserId: user.Id.Value,
            AccessToken: access,
            AccessExpiresAt: accessExp,
            RefreshToken: refreshRaw,
            RefreshExpiresAt: refresh.ExpiresAt
        ));
    }
}