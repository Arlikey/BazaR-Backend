namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record LiqPayWebhookErrorResponse(
    string Code,
    string Message);