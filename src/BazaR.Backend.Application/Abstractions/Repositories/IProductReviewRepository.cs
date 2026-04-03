using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.ProductReviews;

public interface IProductReviewRepository
{
    Task<ProductReview?> GetByIdAsync(ProductReviewId id, CancellationToken ct = default);

    Task<ProductReview?> GetByProductAndAuthorAsync(
        ProductId productId,
        UserId authorUserId,
        CancellationToken ct = default);

    Task AddAsync(ProductReview review, CancellationToken ct = default);
    void Remove(ProductReview review);
}