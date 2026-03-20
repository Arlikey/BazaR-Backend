using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class PaymentProfileRepository : IPaymentProfileRepository
{
    private readonly AppDbContext _db;

    public PaymentProfileRepository(AppDbContext db)
    {
        _db = db;
    }

    public void Add(PaymentProfile profile)
        => _db.PaymentProfiles.Add(profile);

    public void Update(PaymentProfile profile)
        => _db.PaymentProfiles.Update(profile);

    public void Remove(PaymentProfile profile)
        => _db.PaymentProfiles.Remove(profile);

    public async Task<PaymentProfile?> GetByIdAsync(PaymentProfileId id, CancellationToken ct = default)
        => await _db.PaymentProfiles
            .Include(x => x.Methods)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PaymentProfile?> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct = default)
        => await _db.PaymentProfiles
            .Include(x => x.Methods)
            .FirstOrDefaultAsync(x => x.SellerId == sellerId, ct);

    public async Task<PaymentProfile?> GetActiveBySellerIdAsync(SellerId sellerId, CancellationToken ct = default)
        => await _db.PaymentProfiles
            .Include(x => x.Methods)
            .FirstOrDefaultAsync(
                x => x.SellerId == sellerId && x.Status == PaymentProfileStatus.Active,
                ct);

    public async Task<bool> ExistsBySellerIdAsync(SellerId sellerId, CancellationToken ct = default)
        => await _db.PaymentProfiles
            .AnyAsync(x => x.SellerId == sellerId, ct);
}