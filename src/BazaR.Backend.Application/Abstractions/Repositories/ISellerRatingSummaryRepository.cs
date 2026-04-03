using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Domain.Reviews.SellerRatings;

public interface ISellerRatingSummaryRepository
{
    Task<SellerRatingSummary?> GetByIdAsync(SellerId sellerId, CancellationToken ct = default);

    Task AddAsync(SellerRatingSummary summary, CancellationToken ct = default);
}