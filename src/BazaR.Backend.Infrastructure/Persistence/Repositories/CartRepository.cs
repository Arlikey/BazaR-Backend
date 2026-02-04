using BazaR.Backend.Domain.Carts;

using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db) => _db = db;

    public Task<Cart?> GetByUserId(UserId userId)
        => _db.Carts
            .Include("_items")
            .FirstOrDefaultAsync(x => x.UserId == userId);

    public Task Add(Cart cart)
    {
        _db.Carts.Add(cart);
        return Task.CompletedTask;
    }

    public Task Update(Cart cart)
    {
        _db.Carts.Update(cart);
        return Task.CompletedTask;
    }
}
