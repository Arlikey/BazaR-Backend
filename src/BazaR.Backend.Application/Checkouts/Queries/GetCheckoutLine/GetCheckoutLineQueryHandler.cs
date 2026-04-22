using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed class GetCheckoutLineQueryHandler
    : IRequestHandler<GetCheckoutLineQuery, Result<CheckoutLineDto>>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly ICurrentUser _current;

    public GetCheckoutLineQueryHandler(
        ICheckoutRepository checkouts,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _current = current;
    }

    public async Task<Result<CheckoutLineDto>> Handle(
        GetCheckoutLineQuery request,
        CancellationToken ct)
    {
        var checkout = await _checkouts.GetFullByIdAsync(
            new CheckoutId(request.CheckoutId),
            ct);

        if (checkout is null)
        {
            return Result<CheckoutLineDto>.Failure(
                new Error("Checkout.NotFound", "Checkout not found"));
        }

        if (checkout.UserId.Value != _current.UserId)
        {
            return Result<CheckoutLineDto>.Failure(
                new Error("Checkout.Forbidden", "Access denied"));
        }

        var line = checkout.Lines.FirstOrDefault(x => x.Id == new CheckoutLineId(request.LineId));
        if (line is null)
        {
            return Result<CheckoutLineDto>.Failure(
                new Error("CheckoutLine.NotFound", "Line not found"));
        }

        return Result<CheckoutLineDto>.Success(Map(line));
    }

    private static CheckoutLineDto Map(CheckoutLine x)
    {
        var shippingCost = x.Shipping?.Cost.Amount ?? 0m;
        var grandTotal = x.LineTotal.Amount + shippingCost;
        var currency = x.UnitPrice.Currency;

        return new CheckoutLineDto(
            x.Id.Value,
            x.ProductId.Value,
            x.ProductTitle,
            x.Sku,
            x.Quantity,

            x.UnitPrice.Amount,
            x.LineTotal.Amount,
            shippingCost,
            grandTotal,
            currency,

            x.Recipient?.FirstName,
            x.Recipient?.LastName,
            x.Recipient?.Phone,
            x.Recipient?.Email,

            x.Shipping?.MethodType.ToString(),
            x.Shipping?.City,
            x.Shipping?.Region,
            x.Shipping?.PickupPointName,

            x.Payment?.Method.ToString(),
            x.Payment?.RequiresOnlineAuthorization ?? false
        );
    }
}