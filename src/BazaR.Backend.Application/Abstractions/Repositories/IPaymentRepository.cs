using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(PaymentId id, CancellationToken ct = default);
    Task<Payment?> GetByMerchantOrderReferenceAsync(string merchantOrderReference, CancellationToken ct = default);

    Task<IReadOnlyCollection<Payment>> GetByOrderIdAsync(OrderId orderId, CancellationToken ct = default);
    Task<IReadOnlyCollection<Payment>> GetByUserIdAsync(UserId userId, CancellationToken ct = default);
    Task<IReadOnlyCollection<Payment>> GetBySellerIdAsync(SellerId sellerId, CancellationToken ct = default);

    void Add(Payment payment);
    void Update(Payment payment);
}