namespace BazaR.Backend.Api.Contracts.Sellers
{
   
    public sealed record PaymentMethodResponse(
        string MethodType,
        bool IsEnabled,
        bool RequiresOnlineAuthorization,
        bool RequiresBankAccount,
        bool RequiresLiqPay,
        decimal? MinAmount,
        decimal? MaxAmount,
        string? Title,
        string? Description);
}
