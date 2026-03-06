using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.ChangeMyEmail;

public sealed class ChangeMyEmailCommandHandler : IRequestHandler<ChangeMyEmailCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;

    public ChangeMyEmailCommandHandler(IUserRepository users, ICurrentUser current, IUnitOfWork uow)
    {
        _users = users;
        _current = current;
        _uow = uow;
    }

    public async Task<Result> Handle(ChangeMyEmailCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_current.UserId);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found."));

       

        var res = user.ChangeEmail(request.Email);
        if (res.IsFailure) return res;

        _users.Update(user);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}