using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

public sealed class GetCheckoutLineQueryHandler
    : IRequestHandler<GetCheckoutLineQuery, Result<CheckoutLineDto>>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly ISellerRepository _sellers;
    private readonly IProductReadRepository _products;
    private readonly ICurrentUser _current;

    public GetCheckoutLineQueryHandler(
        ICheckoutRepository checkouts,
        ISellerRepository sellers,
        IProductReadRepository products,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _sellers = sellers;
        _products = products;
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

        var seller = await _sellers.GetByIdAsync(line.SellerId, ct);
        var mainImageUrl = await _products.GetMainImageUrlAsync(line.ProductId, ct);

        var dto = Map(line, seller, mainImageUrl);

        return Result<CheckoutLineDto>.Success(dto);
    }

    private static CheckoutLineDto Map(
        CheckoutLine x,
        Seller? seller,
        string? productMainImageUrl)
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
            x.Recipient?.Phone,
            x.Recipient?.Email,

            x.Shipping?.MethodType.ToString(),
            x.Shipping?.City,
            x.Shipping?.Region,
            x.Shipping?.PickupPointName,

            x.Payment?.Method.ToString(),
            x.Payment?.Provider?.ToString(),
            x.Payment?.RequiresOnlineAuthorization ?? false,

            x.SellerId.Value,
            seller?.Name ?? string.Empty,
            productMainImageUrl
        );
    }
}