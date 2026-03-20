using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IPaymentProfileRepository
{
    Task<PaymentProfile?> GetByIdAsync(PaymentProfileId id, CancellationToken ct = default);
    Task<PaymentProfile?> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct = default);
    Task<PaymentProfile?> GetActiveBySellerIdAsync(SellerId sellerId, CancellationToken ct = default);
    Task<bool> ExistsBySellerIdAsync(SellerId sellerId, CancellationToken ct = default);

    void Add(PaymentProfile profile);
    void Update(PaymentProfile profile);
    void Remove(PaymentProfile profile);
}