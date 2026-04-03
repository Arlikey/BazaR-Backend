using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class SellerReviewRepository : ISellerReviewRepository
{
    private readonly AppDbContext _db;

    public SellerReviewRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SellerReview?> GetByIdAsync(
        SellerReviewId id,
        CancellationToken ct = default)
    {
        return await _db.SellerReviews
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<SellerReview?> GetBySellerAndAuthorAsync(
        SellerId sellerId,
        UserId authorUserId,
        CancellationToken ct = default)
    {
        return await _db.SellerReviews
            .FirstOrDefaultAsync(
                x => x.SellerId == sellerId && x.AuthorUserId == authorUserId,
                ct);
    }

    public async Task AddAsync(
        SellerReview review,
        CancellationToken ct = default)
    {
        await _db.SellerReviews.AddAsync(review, ct);
    }

    public void Remove(SellerReview review)
    {
        _db.SellerReviews.Remove(review);
    }
}