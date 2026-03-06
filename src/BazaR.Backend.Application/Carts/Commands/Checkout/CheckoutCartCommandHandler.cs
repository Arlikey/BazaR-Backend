using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.Checkout;

//ДОРАБОТАТЬ СВЯЗЬ С ОРДЕРОМ 
public sealed class CheckoutCartCommandHandler
    : IRequestHandler<CheckoutCartCommand, Result<Guid>>
{
    private readonly ICartRepository _carts;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public CheckoutCartCommandHandler(
        ICartRepository carts,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _carts = carts;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(CheckoutCartCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<Guid>.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_current.UserId);

        var cart = await _carts.GetActiveByUserIdAsync(userId, ct);
        if (cart is null)
            return Result<Guid>.Failure(new Error("Cart.NotFound", "Active cart not found."));

        var res = cart.MarkCheckedOut();
        if (res.IsFailure)
            return Result<Guid>.Failure(res.Error);

        _carts.Update(cart);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(cart.Id.Value);
    }
}