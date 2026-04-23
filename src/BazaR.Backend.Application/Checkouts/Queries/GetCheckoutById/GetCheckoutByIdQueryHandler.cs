using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

public sealed class GetCheckoutByIdQueryHandler
    : IRequestHandler<GetCheckoutByIdQuery, Result<CheckoutDetailsDto>>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly ISellerRepository _sellers;
    private readonly IProductReadRepository _products;
    private readonly ICurrentUser _current;

    public GetCheckoutByIdQueryHandler(
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

    public async Task<Result<CheckoutDetailsDto>> Handle(
        GetCheckoutByIdQuery request,
        CancellationToken ct)
    {
        var checkout = await _checkouts.GetFullByIdAsync(
            new CheckoutId(request.CheckoutId),
            ct);

        if (checkout is null)
            return Result<CheckoutDetailsDto>.Failure(
                new Error("Checkout.NotFound", "Checkout not found"));

        if (checkout.UserId.Value != _current.UserId)
            return Result<CheckoutDetailsDto>.Failure(
                new Error("Checkout.Forbidden", "Access denied"));

        var dto = await MapAsync(checkout, ct);

        return Result<CheckoutDetailsDto>.Success(dto);
    }

    private async Task<CheckoutDetailsDto> MapAsync(Checkout checkout, CancellationToken ct)
    {
        var currency = checkout.Lines.FirstOrDefault()?.UnitPrice.Currency ?? "UAH";

        var lines = new List<CheckoutLineDto>(checkout.Lines.Count);

        foreach (var line in checkout.Lines)
        {
            var seller = await _sellers.GetByIdAsync(line.SellerId, ct);
            var mainImageUrl = await _products.GetMainImageUrlAsync(line.ProductId, ct);

            lines.Add(MapLine(line, seller, mainImageUrl));
        }

        return new CheckoutDetailsDto(
            checkout.Id.Value,
            (int)checkout.Status,

            checkout.ItemsSubtotal.Amount,
            checkout.ShippingTotal.Amount,
            checkout.GrandTotal.Amount,
            currency,

            lines
        );
    }

    private static CheckoutLineDto MapLine(
        CheckoutLine x,
        Seller? seller,
        string? productMainImageUrl)
    {
        var currency = x.UnitPrice.Currency;
        var shippingCost = x.Shipping?.Cost.Amount ?? 0m;
        var grandTotal = x.LineTotal.Amount + shippingCost;

        var recipientName = string.Join(" ",
            new[] { x.Recipient?.FirstName, x.Recipient?.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

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

            string.IsNullOrWhiteSpace(recipientName) ? null : recipientName,
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