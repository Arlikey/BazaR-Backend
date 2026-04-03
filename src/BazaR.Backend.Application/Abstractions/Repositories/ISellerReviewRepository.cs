using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.SellerReviews;

public interface ISellerReviewRepository
{
    Task<SellerReview?> GetByIdAsync(SellerReviewId id, CancellationToken ct = default);

    Task<SellerReview?> GetBySellerAndAuthorAsync(
        SellerId sellerId,
        UserId authorUserId,
        CancellationToken ct = default);

    Task AddAsync(SellerReview review, CancellationToken ct = default);
    void Remove(SellerReview review);
}