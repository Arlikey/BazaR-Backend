using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;

using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.RemoveItem;

public sealed class RemoveCartItemCommandHandler
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly ICartRepository _carts;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;

    public RemoveCartItemCommandHandler(
        ICartRepository carts,
        ICurrentUser current,
        IUnitOfWork uow)
    {
        _carts = carts;
        _current = current;
        _uow = uow;
    }

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        if (request.OfferId == Guid.Empty)
            return Result.Failure(new Error("Offer.InvalidId", "OfferId is invalid."));

        var userId = new UserId(_current.UserId);

        var cart = await _carts.GetActiveByUserIdAsync(userId, ct);
        if (cart is null)
            return Result.Failure(new Error("Cart.NotFound", "Active cart not found."));

        var res = cart.RemoveItem(new OfferId(request.OfferId));
        if (res.IsFailure)
            return res;

        _carts.Update(cart);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}