using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Catalog.Products.Services;

public sealed class ProductOfferAttacher : IProductOfferAttacher
{
    private readonly IOfferReadRepository _offers;
    private readonly ICurrentUser _currentUser;

    public ProductOfferAttacher(IOfferReadRepository offers, ICurrentUser currentUser)
    {
        _offers = offers;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ProductCardWithOfferDto>> AttachOffersAsync(
        IReadOnlyList<ProductCardDto> products,
        CancellationToken ct)
    {
        if (products is null || products.Count == 0)
            return Array.Empty<ProductCardWithOfferDto>();

        var productIds = products.Select(p => p.Id).ToArray();

        UserId? userId = _currentUser.IsAuthenticated
            ? new UserId(_currentUser.UserId)
            : null;

        var offers = await _offers.GetByProductCardIdsAsync(productIds, userId, ct);

        var offerMap = offers.ToDictionary(o => o.ProductId);

        return products
            .Select(p => new ProductCardWithOfferDto(
                Id: p.Id,
                Name: p.Name,
                Slug: p.Slug,
                Description: p.Description,
                MainImageUrl: p.MainImageUrl,
                RatingAverage: p.RatingAverage,
                ReviewsCount: p.ReviewsCount,
                Offer: offerMap.TryGetValue(p.Id, out var offer)
                    ? MapOffer(offer)
                    : null
            ))
            .ToList();
    }

    private static OfferCardDto MapOffer(OfferCardReadDto offer)
        => new(
            Id: offer.OfferId,
            PriceAmount: offer.PriceAmount,
            PriceCurrency: offer.PriceCurrency,
            OldPriceAmount: offer.OldPriceAmount,
            StockQuantity: offer.StockQuantity,
            IsFavorite: offer.IsFavorite
        );
}