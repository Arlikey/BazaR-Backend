namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record StartLiqPayCheckoutResponse(
    Guid PaymentId,
    string ActionUrl,
    string Data,
    string Signature);