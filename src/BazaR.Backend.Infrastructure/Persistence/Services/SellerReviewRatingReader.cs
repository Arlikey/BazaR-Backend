using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Services;
public sealed class SellerReviewRatingReader : ISellerReviewRatingReader
{
    private readonly AppDbContext _db;

    public SellerReviewRatingReader(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<int>> GetApprovedRatingsAsync(
        SellerId sellerId,
        CancellationToken ct = default)
    {
        return await _db.SellerReviews
            .AsNoTracking()
            .Where(x => x.SellerId == sellerId)
            .Where(x => x.Status == ReviewStatus.Approved)
            .Select(x => x.Rating.Value)
            .ToListAsync(ct);
    }
}