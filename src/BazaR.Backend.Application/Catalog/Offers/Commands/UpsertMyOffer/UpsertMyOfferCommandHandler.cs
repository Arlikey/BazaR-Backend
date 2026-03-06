using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sales.Offers.Commands.UpsertMyOffer;

public sealed class UpsertMyOfferCommandHandler : IRequestHandler<UpsertMyOfferCommand, Result>
{
    private readonly IOfferRepository _offers;
    private readonly IProductRepository _products;
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;

    public UpsertMyOfferCommandHandler(
        IOfferRepository offers,
        IProductRepository products,
        ISellerRepository sellers,
        ICurrentUser current,
        IUnitOfWork uow)
    {
        _offers = offers;
        _products = products;
        _sellers = sellers;
        _current = current;
        _uow = uow;
    }

    public async Task<Result> Handle(UpsertMyOfferCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        if (seller.Status != SellerStatus.Active)
            return Result.Failure(new Error("Seller.NotActive", "Seller must be active."));

        var productId = new ProductId(request.ProductId);
        var product = await _products.GetByIdAsync(productId, ct);
        if (product is null)
            return Result.Failure(new Error("Product.NotFound", "Product not found."));

        if (product.OwnerSellerId != seller.Id)
            return Result.Failure(new Error("Auth.Forbidden", "Not your product."));

        // найти оффер (1:1)
        var offer = await _offers.GetByProductAndSellerAsync(productId, seller.Id, ct);

        if (offer is null)
        {
            var createRes = Offer.Create(productId, seller.Id, request.Stock);
            if (createRes.IsFailure) return createRes;

            offer = createRes.Value!;
            _offers.Add(offer);
        }
        else
        {
            _offers.Update(offer);
        }

        // stock
        var stockRes = offer.SetStock(request.Stock);
        if (stockRes.IsFailure) return stockRes;

        // price (если прислали)
        if (request.PriceAmount is not null)
        {
            var currency = string.IsNullOrWhiteSpace(request.PriceCurrency) ? "UAH" : request.PriceCurrency!.Trim().ToUpperInvariant();
            var priceRes = offer.SetPrice(request.PriceAmount.Value, currency);
            if (priceRes.IsFailure) return priceRes;
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
