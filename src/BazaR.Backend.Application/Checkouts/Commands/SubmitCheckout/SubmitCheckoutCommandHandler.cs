using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Checkouts.DTOs;

using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SubmitCheckout;

public sealed class SubmitCheckoutCommandHandler
    : IRequestHandler<SubmitCheckoutCommand, Result<CheckoutSubmissionResult>>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly ICartRepository _carts;
    private readonly ICheckoutSubmissionService _submissionService;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SubmitCheckoutCommandHandler(
        ICheckoutRepository checkouts,
        ICartRepository carts,
        ICheckoutSubmissionService submissionService,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _carts = carts;
        _submissionService = submissionService;
        _uow = uow;
        _current = current;
    }

    public async Task<Result<CheckoutSubmissionResult>> Handle(
        SubmitCheckoutCommand request,
        CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<CheckoutSubmissionResult>.Failure(
                new Error("Auth.Required", "Authentication required."));

        var checkout = await _checkouts.GetByIdAsync(new CheckoutId(request.CheckoutId), ct);
        if (checkout is null)
            return Result<CheckoutSubmissionResult>.Failure(
                new Error("Checkout.NotFound", "Checkout was not found."));

        if (checkout.UserId.Value != _current.UserId)
            return Result<CheckoutSubmissionResult>.Failure(
                new Error("Checkout.Forbidden", "You do not own this checkout."));

        if (!checkout.CanSubmit())
            return Result<CheckoutSubmissionResult>.Failure(
                new Error("Checkout.NotReady", "Checkout is not ready for submission."));

        var submissionResult = await _submissionService.SubmitAsync(
            checkout,
            DateTimeOffset.UtcNow,
            ct);

        if (submissionResult.IsFailure)
            return Result<CheckoutSubmissionResult>.Failure(submissionResult.Error);

        checkout.Submit(DateTime.UtcNow);

        var cart = await _carts.GetByIdAsync(checkout.CartId, ct);
        if (cart is not null)
        {
            var cartResult = cart.MarkCheckedOut(DateTimeOffset.UtcNow);
            if (cartResult.IsFailure)
                return Result<CheckoutSubmissionResult>.Failure(cartResult.Error);

            _carts.Update(cart);
        }

        _checkouts.Update(checkout);
        await _uow.SaveChangesAsync(ct);

        return Result<CheckoutSubmissionResult>.Success(submissionResult.Value!);
    }
}