using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Carts;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class CheckoutRepository : ICheckoutRepository
{
    private readonly AppDbContext _db;

    public CheckoutRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Checkout?> GetByIdAsync(CheckoutId id, CancellationToken ct)
    {
        return await _db.Checkouts
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<Checkout?> GetDraftByCartIdAsync(CartId cartId, CancellationToken ct)
    {
        return await _db.Checkouts
            .FirstOrDefaultAsync(
                x => x.CartId == cartId && x.Status == CheckoutStatus.Draft,
                ct);
    }

    public async Task<Checkout?> GetActiveDraftByUserIdAsync(UserId userId, CancellationToken ct)
    {
        return await _db.Checkouts
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.Status == CheckoutStatus.Draft,
                ct);
    }

    public void Add(Checkout checkout)
    {
        _db.Checkouts.Add(checkout);
    }

    public void Update(Checkout checkout)
    {
        _db.Checkouts.Update(checkout);
    }
}