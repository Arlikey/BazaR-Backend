using BazaR.Backend.Domain.Orders;

namespace BazaR.Backend.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetById(OrderId id);
    Task Add(Order order);
    Task Update(Order order);
}
