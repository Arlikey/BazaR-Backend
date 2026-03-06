using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Application.Catalog.Products.DTOs;

namespace BazaR.Backend.Application.Catalog.Products.Services;

public sealed class ProductOfferAttacher : IProductOfferAttacher
{
    private readonly IOfferReadRepository _offers;

    public ProductOfferAttacher(IOfferReadRepository offers)
    {
        _offers = offers;
    }

    public async Task<IReadOnlyList<ProductCardWithOfferDto>> AttachOffersAsync(
        IReadOnlyList<ProductCardDto> products,
        CancellationToken ct)
    {
        if (products is null || products.Count == 0)
            return Array.Empty<ProductCardWithOfferDto>();

        var productIds = products.Select(p => p.Id).ToArray();

       
        var offers = await _offers.GetByProductCardIdsAsync(productIds, ct);

     
        var offerMap = offers.ToDictionary(o => o.ProductId);

        return products
            .Select(p => new ProductCardWithOfferDto(
                Id: p.Id,
                Name: p.Name,
                Slug: p.Slug,
                Description: p.Description,
                MainImageUrl: p.MainImageUrl,
                Offer: offerMap.TryGetValue(p.Id, out var offer) ? offer : null
            ))
            .ToList();
    }
}