using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.RemoveMyAvatar;

public sealed record RemoveMyAvatarCommand() : IRequest<Result>;

public sealed class RemoveMyAvatarCommandHandler : IRequestHandler<RemoveMyAvatarCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;

    public RemoveMyAvatarCommandHandler(
        IUserRepository users,
        ICurrentUser current,
        IUnitOfWork uow,
        IFileStorage files)
    {
        _users = users;
        _current = current;
        _uow = uow;
        _files = files;
    }

    public async Task<Result> Handle(RemoveMyAvatarCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_current.UserId);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found."));

        var key = user.Avatar?.StorageKey;
        var res = user.RemoveAvatar();
        if (res.IsFailure) return res;

        _users.Update(user);
        await _uow.SaveChangesAsync(ct);

        if (!string.IsNullOrWhiteSpace(key))
        {
            try { await _files.DeleteAsync(key, ct); } catch { }
        }

        return Result.Success();
    }
}