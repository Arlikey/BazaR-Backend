/*using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sales.Offers.Queries.GetMyOfferByProductId;

public sealed class GetMyOfferByProductIdQueryHandler
    : IRequestHandler<GetMyOfferByProductIdQuery, Result<MyOfferDto>>
{
    private readonly IOfferRepository _offers;
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;

    public GetMyOfferByProductIdQueryHandler(
        IOfferRepository offers,
        ISellerRepository sellers,
        ICurrentUser current)
    {
        _offers = offers;
        _sellers = sellers;
        _current = current;
    }

    public async Task<Result<MyOfferDto>> Handle(GetMyOfferByProductIdQuery request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<MyOfferDto>.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<MyOfferDto>.Failure(new Error("Seller.NotFound", "Seller not found."));

        var productId = new ProductId(request.ProductId);

        var offer = await _offers.GetByProductAndSellerAsync(productId, seller.Id, ct);
        if (offer is null)
            return Result<MyOfferDto>.Failure(new Error("Offer.NotFound", "Offer not found."));

        var dto = new MyOfferDto(
            ProductId: offer.ProductId.Value,
            SellerId: offer.SellerId.Value,
            PriceAmount: offer.Price?.Amount,
            PriceCurrency: offer.Price?.Currency,
            Stock: offer.Stock,
            Status: offer.Status.ToString()
        );

        return Result<MyOfferDto>.Success(dto);
    }
}
*/