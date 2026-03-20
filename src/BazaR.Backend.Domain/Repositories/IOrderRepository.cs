using BazaR.Backend.Domain.Orders;

namespace BazaR.Backend.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default);
    Task Add(Order order);
    Task Update(Order order);
}
