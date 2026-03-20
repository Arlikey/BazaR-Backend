using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Checkouts.Services;
using BazaR.Backend.Application.Abstractions.Checkouts;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Domain.Catalog.Products;

public sealed class CheckoutSnapshotBuilder : ICheckoutSnapshotBuilder
{
    private readonly IOfferSnapshotReader _offers;

    public CheckoutSnapshotBuilder(IOfferSnapshotReader offers)
    {
        _offers = offers;
    }

    public async Task<Result<IReadOnlyCollection<CheckoutLineSnapshotData>>> BuildAsync(
        Cart cart,
        CancellationToken ct)
    {
        var result = new List<CheckoutLineSnapshotData>();

        foreach (var item in cart.Items)
        {
            var offer = await _offers.GetByIdAsync(item.OfferId, ct);
            if (offer is null)
            {
                return Result<IReadOnlyCollection<CheckoutLineSnapshotData>>.Failure(
                    new Error("Offer.NotFound", $"Offer '{item.OfferId.Value}' was not found."));
            }

            var moneyResult = Money.Create(item.PriceSnapshot.Amount, item.PriceSnapshot.Currency);
            if (moneyResult.IsFailure)
                return Result<IReadOnlyCollection<CheckoutLineSnapshotData>>.Failure(moneyResult.Error);

            result.Add(new CheckoutLineSnapshotData(
                item.OfferId,
                new ProductId(offer.ProductId),
                new SellerId(offer.SellerId),
                offer.ProductTitle,
                offer.Sku,
                item.Quantity,
                moneyResult.Value!));
        }

        return Result<IReadOnlyCollection<CheckoutLineSnapshotData>>.Success(result);
    }
}
