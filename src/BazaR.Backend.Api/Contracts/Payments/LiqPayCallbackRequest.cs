namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record LiqPayCallbackRequest(
    string Data,
    string Signature);