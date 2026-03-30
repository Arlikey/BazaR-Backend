using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Shippings;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ShippingRepository : IShippingRepository
{
    private readonly AppDbContext _db;

    public ShippingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Shipping?> GetByIdAsync(
        ShippingId id,
        CancellationToken ct = default)
    {
        return await _db.Shippings
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Shipping?> GetByOrderIdAsync(
        OrderId orderId,
        CancellationToken ct = default)
    {
        return await _db.Shippings
            .FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
    }

    public async Task<bool> ExistsByOrderIdAsync(
        OrderId orderId,
        CancellationToken ct = default)
    {
        return await _db.Shippings
            .AnyAsync(x => x.OrderId == orderId, ct);
    }

    public async Task AddAsync(
        Shipping shipping,
        CancellationToken ct = default)
    {
        await _db.Shippings.AddAsync(shipping, ct);
    }

    public void Update(Shipping shipping)
    {
        _db.Shippings.Update(shipping);
    }

    public void Remove(Shipping shipping)
    {
        _db.Shippings.Remove(shipping);
    }
}