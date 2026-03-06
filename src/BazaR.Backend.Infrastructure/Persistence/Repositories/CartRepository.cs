// Infrastructure/Persistence/Repositories/CartRepository.cs
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;
    public CartRepository(AppDbContext db) => _db = db;

    public async Task<Cart?> GetByIdAsync(CartId id, CancellationToken ct = default)
        => await _db.Carts
            .Include("_items") 
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Cart?> GetByUserIdAsync(UserId userId, CancellationToken ct = default)
        => await _db.Carts
            .Include("_items")
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task<Cart?> GetActiveByUserIdAsync(UserId userId, CancellationToken ct = default)
        => await _db.Carts
            .Include("_items")
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == CartStatus.Active, ct);

    public void Add(Cart cart) => _db.Carts.Add(cart);
    public void Update(Cart cart) => _db.Carts.Update(cart);
}