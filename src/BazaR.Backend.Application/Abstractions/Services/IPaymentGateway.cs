/*using BazaR.Backend.Application.Payments.DTOs;

namespace BazaR.Backend.Application.Abstractions.Services;

public interface IPaymentGateway
{
    Task<CreateCheckoutResult> CreateCheckoutAsync(
        CreateCheckoutRequest request,
        CancellationToken ct);

    Task<PaymentStatusCheckResult> GetStatusAsync(
        PaymentStatusCheckRequest request,
        CancellationToken ct);

    Task<RefundPaymentResult> RefundAsync(
        RefundPaymentRequest request,
        CancellationToken ct);

    Task<ParsedPaymentCallback> ParseCallbackAsync(
        string payload,
        IReadOnlyDictionary<string, string> headers,
        CancellationToken ct);
}*/