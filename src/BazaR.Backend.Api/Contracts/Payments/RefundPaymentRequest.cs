namespace BazaR.Backend.Api.Contracts.Payments;

public sealed record RefundPaymentRequest(
    decimal Amount,
    string Currency);