using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions.Security;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Identity.Commands.Logout;

public sealed record LogoutCommand(Guid UserId, string RefreshToken) : IRequest<Result>;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IAuthUserRepository _authUsers;
    private readonly IJwtProvider _jwt;
    private readonly IUnitOfWork _uow;

    public LogoutCommandHandler(IAuthUserRepository authUsers, IJwtProvider jwt, IUnitOfWork uow)
    {
        _authUsers = authUsers;
        _jwt = jwt;
        _uow = uow;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var auth = await _authUsers.GetByUserIdAsync(request.UserId, ct);
        if (auth is null) return Result.Success();

        var hash = _jwt.HashRefreshToken(request.RefreshToken);
        var rt = auth.RefreshTokens.FirstOrDefault(x => x.IsActive && x.TokenHash == hash);
        if (rt is null) return Result.Success();

        auth.RevokeRefreshToken(rt.Id);
        _authUsers.Update(auth);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
