using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions.Security;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Identity;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Identity.Commands.Refresh;

public sealed record RefreshTokenCommand(Guid UserId, string RefreshToken) : IRequest<Result<AuthResponse>>;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IAuthUserRepository _authUsers;
    private readonly IUserRepository _users;
    private readonly IJwtProvider _jwt;
    private readonly IUnitOfWork _uow;

    private static readonly TimeSpan RefreshTtl = TimeSpan.FromDays(30);

    public RefreshTokenCommandHandler(IAuthUserRepository authUsers, IUserRepository users, IJwtProvider jwt, IUnitOfWork uow)
    {
        _authUsers = authUsers;
        _users = users;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        if (request.UserId == Guid.Empty || string.IsNullOrWhiteSpace(request.RefreshToken))
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidRefresh", "Invalid refresh token."));

        var auth = await _authUsers.GetByUserIdAsync(request.UserId, ct);
        if (auth is null)
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidRefresh", "Invalid refresh token."));

        var refreshHash = _jwt.HashRefreshToken(request.RefreshToken);
        var existing = auth.FindActiveRefreshByHash(refreshHash);
        if (existing is null)
            return Result<AuthResponse>.Failure(new Error("Auth.InvalidRefresh", "Invalid refresh token."));

       
        auth.RevokeRefreshToken(existing.Id);

        var user = await _users.GetByIdAsync(new UserId(auth.UserId), ct);
        if (user is null)
            return Result<AuthResponse>.Failure(new Error("User.NotFound", "User not found."));

        var roles = user.Roles.Select(r => r.ToString()).ToList();
        var (access, accessExp) = _jwt.CreateAccessToken(user.Id.Value, roles);

        var newRefreshRaw = _jwt.CreateRefreshToken();
        var newRefreshHash = _jwt.HashRefreshToken(newRefreshRaw);
        var newRefresh = auth.IssueRefreshToken(newRefreshHash, RefreshTtl);

        _authUsers.Update(auth);
        await _uow.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(new AuthResponse(
            UserId: user.Id.Value,
            AccessToken: access,
            AccessExpiresAt: accessExp,
            RefreshToken: newRefreshRaw,
            RefreshExpiresAt: newRefresh.ExpiresAt
        ));
    }
}
