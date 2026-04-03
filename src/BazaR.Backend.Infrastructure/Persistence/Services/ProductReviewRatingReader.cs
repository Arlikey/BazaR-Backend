using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace BazaR.Backend.Infrastructure.Persistence.Services;

public sealed class ProductReviewRatingReader : IProductReviewRatingReader
{
    private readonly AppDbContext _db;

    public ProductReviewRatingReader(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<int>> GetApprovedRatingsAsync(
        ProductId productId,
        CancellationToken ct = default)
    {
        return await _db.ProductReviews
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .Where(x => x.Status == ReviewStatus.Approved)
            .Select(x => x.Rating.Value)
            .ToListAsync(ct);
    }
}