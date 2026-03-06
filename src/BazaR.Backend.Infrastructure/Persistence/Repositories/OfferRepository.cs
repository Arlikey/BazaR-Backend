using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class OfferRepository : IOfferRepository
{
    private readonly AppDbContext _db;

    public OfferRepository(AppDbContext db) => _db = db;

    public void Add(Offer offer) => _db.Offers.Add(offer);

    public void Update(Offer offer) => _db.Offers.Update(offer);

    public async Task<Offer?> GetByIdAsync(OfferId id, CancellationToken cancellationToken = default)
        => await _db.Offers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Offer?> GetByProductAndSellerAsync(
        ProductId productId,
        SellerId sellerId,
        CancellationToken cancellationToken = default)
        => await _db.Offers
            .FirstOrDefaultAsync(x => x.ProductId == productId && x.SellerId == sellerId, cancellationToken);
}