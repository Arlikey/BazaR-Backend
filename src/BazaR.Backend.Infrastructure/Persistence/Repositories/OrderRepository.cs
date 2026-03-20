using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db) => _db = db;

    public Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken = default)
     => _db.Orders
         .Include("_items")
         .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task Add(Order order)
    {
        _db.Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task Update(Order order)
    {
        _db.Orders.Update(order);
        return Task.CompletedTask;
    }
}
