using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLineRecipient;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed class SetCheckoutLineRecipientCommandHandler
    : IRequestHandler<SetCheckoutLineRecipientCommand, Result>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetCheckoutLineRecipientCommandHandler(
        ICheckoutRepository checkouts,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SetCheckoutLineRecipientCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var checkout = await _checkouts.GetByIdAsync(new CheckoutId(request.CheckoutId), ct);
        if (checkout is null)
            return Result.Failure(new Error("Checkout.NotFound", "Checkout not found."));

        if (checkout.UserId.Value != _current.UserId)
            return Result.Failure(new Error("Checkout.Forbidden", "You do not own this checkout."));

        var recipient = RecipientInfo.Create(
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Email,
            request.IsCustomerRecipient);

        checkout.SetLineRecipient(
            new CheckoutLineId(request.LineId),
            recipient,
            DateTime.UtcNow);

        _checkouts.Update(checkout);

        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}