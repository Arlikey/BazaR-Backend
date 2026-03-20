namespace BazaR.Backend.Application.Payments.DTOs;

public sealed record LiqPayCheckoutStartResult(
    Guid PaymentId,
    string ActionUrl,
    string Data,
    string Signature);