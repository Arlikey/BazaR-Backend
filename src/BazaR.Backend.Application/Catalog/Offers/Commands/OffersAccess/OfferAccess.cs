using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Offers;

internal static class OfferAccess
{
    public static async Task<Result<(Seller seller, Offer offer)>> GetMySellerAndOfferAsync(
        Guid offerId,
        ICurrentUser current,
        ISellerRepository sellers,
        IOfferRepository offers,
        CancellationToken ct)
    {
        if (!current.IsAuthenticated)
            return Result<(Seller, Offer)>.Failure(new Error("Auth.Required", "Authentication required."));

        if (offerId == Guid.Empty)
            return Result<(Seller, Offer)>.Failure(new Error("Offer.IdRequired", "OfferId is required."));

        var seller = await sellers.GetByOwnerUserIdAsync(current.UserId, ct);
        if (seller is null)
            return Result<(Seller, Offer)>.Failure(new Error("Seller.NotFound", "Seller not found."));

        var offer = await offers.GetByIdAsync(new OfferId(offerId), ct);
        if (offer is null)
            return Result<(Seller, Offer)>.Failure(new Error("Offer.NotFound", "Offer not found."));

        if (offer.SellerId != seller.Id)
            return Result<(Seller, Offer)>.Failure(new Error("Offer.Forbidden", "Offer does not belong to current seller."));

        return Result<(Seller, Offer)>.Success((seller, offer));
    }
}