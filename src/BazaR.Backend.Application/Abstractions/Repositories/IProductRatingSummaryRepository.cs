using BazaR.Backend.Domain.Catalog.Products;

namespace BazaR.Backend.Domain.Reviews.ProductRatings;

public interface IProductRatingSummaryRepository
{
    Task<ProductRatingSummary?> GetByIdAsync(ProductId productId, CancellationToken ct = default);

    Task AddAsync(ProductRatingSummary summary, CancellationToken ct = default);
}