using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shipping;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IShippingProfileRepository
{
    Task<ShippingProfile?> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct);
    Task<ShippingProfile?> GetActiveBySellerIdAsync(SellerId sellerId, CancellationToken ct);
    void Add(ShippingProfile profile);
    void Update(ShippingProfile profile);
}