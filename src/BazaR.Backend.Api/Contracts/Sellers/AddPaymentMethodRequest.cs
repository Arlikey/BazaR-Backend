using BazaR.Backend.Domain.PaymentProfiles;

namespace BazaR.Backend.Api.Contracts.Sellers;

public sealed record AddPaymentMethodRequest(
    PaymentMethodType MethodType,
    bool RequiresOnlineAuthorization,
    bool RequiresBankAccount,
    bool RequiresLiqPay,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? Title,
    string? Description);