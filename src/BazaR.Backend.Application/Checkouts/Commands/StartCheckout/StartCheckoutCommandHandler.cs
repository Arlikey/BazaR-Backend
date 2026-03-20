using BazaR.Backend.Application.Abstractions.Checkouts;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.StartCheckout;

public sealed class StartCheckoutCommandHandler : IRequestHandler<StartCheckoutCommand, Result<Guid>>
{
    private readonly ICartRepository _carts;
    private readonly ICheckoutRepository _checkouts;
    private readonly ICheckoutSnapshotBuilder _snapshotBuilder;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public StartCheckoutCommandHandler(
        ICartRepository carts,
        ICheckoutRepository checkouts,
        ICheckoutSnapshotBuilder snapshotBuilder,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _carts = carts;
        _checkouts = checkouts;
        _snapshotBuilder = snapshotBuilder;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<Guid>> Handle(StartCheckoutCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<Guid>.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_current.UserId);

        var cart = await _carts.GetActiveByUserIdAsync(userId, ct);
        if (cart is null)
            return Result<Guid>.Failure(new Error("Cart.NotFound", "Active cart was not found."));

        if (!cart.Items.Any())
            return Result<Guid>.Failure(new Error("Checkout.EmptyCart", "Cannot start checkout from empty cart."));

        var existingDraft = await _checkouts.GetDraftByCartIdAsync(cart.Id, ct);
        if (existingDraft is not null)
            return Result<Guid>.Success(existingDraft.Id.Value);

        var snapshotsResult = await _snapshotBuilder.BuildAsync(cart, ct);
        if (snapshotsResult.IsFailure)
            return Result<Guid>.Failure(snapshotsResult.Error);

        var checkout = Checkout.Start(
            CheckoutId.New(),
            cart.Id,
            userId,
            snapshotsResult.Value!,
            DateTime.UtcNow);

        var cartResult = cart.MarkCheckedOut(DateTimeOffset.UtcNow);
        if (cartResult.IsFailure)
            return Result<Guid>.Failure(cartResult.Error);

        _checkouts.Add(checkout);
        _carts.Update(cart);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(checkout.Id.Value);
    }
}