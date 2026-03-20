using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record CreatePaymentRequest(
    Guid OrderId,
    Guid SellerId,
    Guid UserId,
    PaymentProvider Provider,
    PaymentMethod Method,
    decimal Amount,
    string Currency,
    string MerchantOrderReference);