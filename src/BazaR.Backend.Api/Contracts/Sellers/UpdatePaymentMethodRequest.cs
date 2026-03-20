namespace BazaR.Backend.Api.Contracts.Sellers;

public sealed record UpdatePaymentMethodRequest(
    bool RequiresOnlineAuthorization,
    bool RequiresBankAccount,
    bool RequiresLiqPay,
    decimal? MinAmount,
    decimal? MaxAmount,
    string? Title,
    string? Description);