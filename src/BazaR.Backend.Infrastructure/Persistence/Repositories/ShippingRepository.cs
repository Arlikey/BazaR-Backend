using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.Shippings;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ShippingRepository : IShippingRepository
{
    private readonly AppDbContext _db;

    public ShippingRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Shipping?> GetByIdAsync(ShippingId id, CancellationToken ct)
    {
        return await _db.Shippings
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Shipping?> GetByOrderIdAsync(OrderId orderId, CancellationToken ct)
    {
        return await _db.Shippings
            .FirstOrDefaultAsync(x => x.OrderId == orderId, ct);
    }

    public void Add(Shipping shipping)
    {
        _db.Shippings.Add(shipping);
    }

    public void Update(Shipping shipping)
    {
        _db.Shippings.Update(shipping);
    }
}