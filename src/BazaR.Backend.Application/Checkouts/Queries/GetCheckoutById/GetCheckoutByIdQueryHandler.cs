using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed class GetCheckoutByIdQueryHandler
    : IRequestHandler<GetCheckoutByIdQuery, Result<CheckoutDetailsDto>>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly ICurrentUser _current;

    public GetCheckoutByIdQueryHandler(
        ICheckoutRepository checkouts,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _current = current;
    }

    public async Task<Result<CheckoutDetailsDto>> Handle(
        GetCheckoutByIdQuery request,
        CancellationToken ct)
    {
        var checkout = await _checkouts.GetFullByIdAsync(
            new CheckoutId(request.CheckoutId), ct);

        if (checkout is null)
            return Result<CheckoutDetailsDto>.Failure(
                new Error("Checkout.NotFound", "Checkout not found"));

        if (checkout.UserId.Value != _current.UserId)
            return Result<CheckoutDetailsDto>.Failure(
                new Error("Checkout.Forbidden", "Access denied"));

        var dto = Map(checkout);

        return Result<CheckoutDetailsDto>.Success(dto);
    }

    private static CheckoutDetailsDto Map(Checkout checkout)
    {
        var currency = checkout.Lines.FirstOrDefault()?.UnitPrice.Currency ?? "UAH";

        return new CheckoutDetailsDto(
            checkout.Id.Value,
            (int)checkout.Status,

            checkout.ItemsSubtotal.Amount,
            checkout.ShippingTotal.Amount,
            checkout.GrandTotal.Amount,
            currency,

            checkout.Lines.Select(MapLine).ToList()
        );
    }

    private static CheckoutLineDto MapLine(CheckoutLine x)
    {
        var currency = x.UnitPrice.Currency;

        var shippingCost = x.Shipping?.Cost.Amount ?? 0m;
        var grandTotal = x.LineTotal.Amount + shippingCost;

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