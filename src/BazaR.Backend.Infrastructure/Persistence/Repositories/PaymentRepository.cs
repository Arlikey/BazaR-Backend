using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;

    public PaymentRepository(AppDbContext db)
    {
        _db = db;
    }

    public void Add(Payment payment)
        => _db.Payments.Add(payment);

    public void Update(Payment payment)
        => _db.Payments.Update(payment);

    public async Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default)
        => await _db.Payments
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Payment?> GetByMerchantOrderReferenceAsync(string merchantOrderReference, CancellationToken ct = default)
    {
        var normalized = NormalizeMerchantOrderReference(merchantOrderReference);
        if (normalized is null)
            return null;

        return await _db.Payments
            .FirstOrDefaultAsync(x => x.MerchantOrderReference == normalized, ct);
    }

    public async Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(OrderId orderId, CancellationToken ct = default)
        => await _db.Payments
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<Payment>> GetByUserIdAsync(UserId userId, CancellationToken ct = default)
        => await _db.Payments
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task<IReadOnlyCollection<Payment>> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct = default)
        => await _db.Payments
            .Where(x => x.SellerId == sellerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);

    private static string? NormalizeMerchantOrderReference(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}