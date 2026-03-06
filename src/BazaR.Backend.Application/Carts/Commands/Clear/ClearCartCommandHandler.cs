using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.Clear;

public sealed class ClearCartCommandHandler
    : IRequestHandler<ClearCartCommand, Result>
{
    private readonly ICartRepository _carts;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;

    public ClearCartCommandHandler(
        ICartRepository carts,
        ICurrentUser current,
        IUnitOfWork uow)
    {
        _carts = carts;
        _current = current;
        _uow = uow;
    }

    public async Task<Result> Handle(ClearCartCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_current.UserId);

        var cart = await _carts.GetActiveByUserIdAsync(userId, ct);
        if (cart is null)
            return Result.Success();

        var res = cart.Clear();
        if (res.IsFailure)
            return res;

        _carts.Update(cart);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}