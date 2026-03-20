using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.Shippings;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IShippingRepository
{
    Task<Shipping?> GetByIdAsync(ShippingId id, CancellationToken ct);
    Task<Shipping?> GetByOrderIdAsync(OrderId orderId, CancellationToken ct);
    //Task<IReadOnlyCollection<Shipping>> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct = default);
    void Add(Shipping shipping);
    void Update(Shipping shipping);
}