using BazaR.Backend.Domain.Reviews.SellerRatings;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class SellerRatingSummaryRepository : ISellerRatingSummaryRepository
{
    private readonly AppDbContext _db;

    public SellerRatingSummaryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SellerRatingSummary?> GetByIdAsync(
        SellerId sellerId,
        CancellationToken ct = default)
    {
        return await _db.SellerRatingSummaries
            .FirstOrDefaultAsync(x => x.Id == sellerId, ct);
    }

    public async Task AddAsync(
        SellerRatingSummary summary,
        CancellationToken ct = default)
    {
        await _db.SellerRatingSummaries.AddAsync(summary, ct);
    }
}