using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Sales;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Services;

public sealed class OfferSnapshotReader : IOfferSnapshotReader
{
    private readonly AppDbContext _db;

    public OfferSnapshotReader(AppDbContext db)
    {
        _db = db;
    }

    public async Task<OfferSnapshotDto?> GetByIdAsync(OfferId offerId, CancellationToken ct)
    {
        var offerRow = await _db.Offers
            .Where(x => x.Id == offerId)
            .Select(x => new
            {
                x.Id,
                ProductId = x.ProductId.Value,
                SellerId = x.SellerId.Value,
                x.SellerSku
            })
            .FirstOrDefaultAsync(ct);

        if (offerRow is null)
            return null;

        var productId = new ProductId(offerRow.ProductId);

        var productTitle = await _db.Products
            .Where(x => x.Id == productId)
            .Select(x => x.Name)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(productTitle))
            return null;

        return new OfferSnapshotDto(
            offerRow.Id.Value,
            offerRow.ProductId,
            offerRow.SellerId,
            productTitle,
            offerRow.SellerSku);
    }
}