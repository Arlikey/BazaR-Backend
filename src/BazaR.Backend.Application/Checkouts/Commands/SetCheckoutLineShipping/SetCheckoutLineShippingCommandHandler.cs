using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLineShipping;

public sealed class SetCheckoutLineShippingCommandHandler
    : IRequestHandler<SetCheckoutLineShippingCommand, Result>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly IShippingProfileRepository _shippingProfiles;
    private readonly IShippingSelectionFactory _shippingSelectionFactory;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetCheckoutLineShippingCommandHandler(
        ICheckoutRepository checkouts,
        IShippingProfileRepository shippingProfiles,
        IShippingSelectionFactory shippingSelectionFactory,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _shippingProfiles = shippingProfiles;
        _shippingSelectionFactory = shippingSelectionFactory;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SetCheckoutLineShippingCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
        {
            return Result.Failure(new Error(
                "Auth.Required",
                "Authentication required."));
        }

        var checkout = await _checkouts.GetByIdAsync(new CheckoutId(request.CheckoutId), ct);
        if (checkout is null)
        {
            return Result.Failure(new Error(
                "Checkout.NotFound",
                "Checkout was not found."));
        }

        if (checkout.UserId.Value != _current.UserId)
        {
            return Result.Failure(new Error(
                "Checkout.Forbidden",
                "You do not own this checkout."));
        }

        var lineId = new CheckoutLineId(request.LineId);
        var line = checkout.Lines.FirstOrDefault(x => x.Id == lineId);
        if (line is null)
        {
            return Result.Failure(new Error(
                "Checkout.Line.NotFound",
                "Checkout line was not found."));
        }

        

        var profile = await _shippingProfiles.GetActiveBySellerIdAsync(line.SellerId, ct);
        if (profile is null)
        {
            return Result.Failure(new Error(
                "ShippingProfile.NotFound",
                "Active shipping profile for seller was not found."));
        }

        var selectionResult = _shippingSelectionFactory.Create(
            profile,
            request.Method,
            request.Country,
            request.Region,
            request.City,
            request.Street,
            request.House,
            request.Apartment,
            request.PostalCode,
            request.WarehouseCode,
            request.WarehouseName,
            request.Comment);

        if (selectionResult.IsFailure)
            return Result.Failure(selectionResult.Error);

        var shipping = selectionResult.Value!;

        if (shipping.Cost.Currency != line.UnitPrice.Currency)
        {
            return Result.Failure(new Error(
                "Checkout.Line.Shipping.CurrencyMismatch",
                $"Shipping currency '{shipping.Cost.Currency}' must match line currency '{line.UnitPrice.Currency}'."));
        }

        checkout.SetLineShipping(
            lineId,
            shipping,
            DateTime.UtcNow);

        _checkouts.Update(checkout);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}