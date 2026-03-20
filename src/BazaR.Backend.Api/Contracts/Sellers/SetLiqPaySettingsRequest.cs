namespace BazaR.Backend.Api.Contracts.Sellers
{
   public sealed record SetLiqPaySettingsRequest(
        string PublicKey,
        string PrivateKey,
        string? ResultUrl,
        string? ServerCallbackUrl,
        bool CheckoutEnabled,
        bool PrivatPayEnabled,
        bool InstallmentsEnabled);
}
