namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record CancelPaymentRequest(
    string? ExternalStatus);