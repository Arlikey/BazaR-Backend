using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed class GetOfferByProductIdQueryHandler
    : IRequestHandler<GetOfferByProductIdQuery, Result<OfferDetailsDto?>>
{
    private readonly IOfferReadRepository _offers;

    public GetOfferByProductIdQueryHandler(IOfferReadRepository offers)
        => _offers = offers;

    public async Task<Result<OfferDetailsDto?>> Handle(GetOfferByProductIdQuery request, CancellationToken ct)
    {
        if (request.ProductId == Guid.Empty)
            return Result<OfferDetailsDto?>
                .Failure(new Error("Product.InvalidId", "ProductId is invalid."));

        var productId = new ProductId(request.ProductId);

        var offer = await _offers.GetByProductIdAsync(productId, ct);

       
        return Result<OfferDetailsDto?>.Success(offer);
    }
}