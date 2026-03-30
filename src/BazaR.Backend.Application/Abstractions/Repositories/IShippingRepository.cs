using BazaR.Backend.Domain.Orders;

namespace BazaR.Backend.Domain.Shippings;

public interface IShippingRepository
{
    Task<Shipping?> GetByIdAsync(ShippingId id, CancellationToken ct = default);
    Task<Shipping?> GetByOrderIdAsync(OrderId orderId, CancellationToken ct = default);
    Task<bool> ExistsByOrderIdAsync(OrderId orderId, CancellationToken ct = default);

    Task AddAsync(Shipping shipping, CancellationToken ct = default);
    void Update(Shipping shipping);
    void Remove(Shipping shipping);
}