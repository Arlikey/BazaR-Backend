using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Reviews.ProductRatings;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ProductRatingSummaryRepository : IProductRatingSummaryRepository
{
    private readonly AppDbContext _db;

    public ProductRatingSummaryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProductRatingSummary?> GetByIdAsync(
        ProductId productId,
        CancellationToken ct = default)
    {
        return await _db.ProductRatingSummaries
            .FirstOrDefaultAsync(x => x.Id == productId, ct);
    }

    public async Task AddAsync(
        ProductRatingSummary summary,
        CancellationToken ct = default)
    {
        await _db.ProductRatingSummaries.AddAsync(summary, ct);
    }
}