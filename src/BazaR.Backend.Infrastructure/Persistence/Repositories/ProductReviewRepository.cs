using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class ProductReviewRepository : IProductReviewRepository
{
    private readonly AppDbContext _db;

    public ProductReviewRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProductReview?> GetByIdAsync(
        ProductReviewId id,
        CancellationToken ct = default)
    {
        return await _db.ProductReviews
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<ProductReview?> GetByProductAndAuthorAsync(
        ProductId productId,
        UserId authorUserId,
        CancellationToken ct = default)
    {
        return await _db.ProductReviews
            .FirstOrDefaultAsync(
                x => x.ProductId == productId && x.AuthorUserId == authorUserId,
                ct);
    }

    public async Task AddAsync(
        ProductReview review,
        CancellationToken ct = default)
    {
        await _db.ProductReviews.AddAsync(review, ct);
    }

    public void Remove(ProductReview review)
    {
        _db.ProductReviews.Remove(review);
    }
}