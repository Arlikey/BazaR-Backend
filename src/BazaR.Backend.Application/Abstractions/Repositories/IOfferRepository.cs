using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Sales;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IOfferRepository
{
    Task<Offer?> GetByIdAsync(OfferId id, CancellationToken cancellationToken = default);
    Task<Offer?> GetByProductAndSellerAsync(ProductId productId, SellerId sellerId, CancellationToken cancellationToken = default);
    void Add(Offer offer);
    void Update(Offer offer);
}