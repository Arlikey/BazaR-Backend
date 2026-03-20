/*using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Application.Payments.DTOs;

public sealed record CreateCheckoutRequest(
    Guid PaymentId,
    Guid OrderId,
    string MerchantOrderReference,
    decimal Amount,
    string Currency,
    string Description,
    string CustomerEmail,
    string ResultUrl,
    string ServerUrl,
    PaymentProvider Provider);

public sealed record CreateCheckoutResult(
    string CheckoutActionUrl,
    string Data,
    string Signature,
    string? ExternalOrderReference,
    string? ExternalSessionId,
    string? ExternalStatus);

public sealed record PaymentStatusCheckRequest(
    PaymentProvider Provider,
    string MerchantOrderReference,
    string? ExternalPaymentId,
    string? ExternalOrderReference);

public sealed record PaymentStatusCheckResult(
    string? ExternalPaymentId,
    string? ExternalStatus,
    bool IsAuthorized,
    bool IsPaid,
    bool IsFailed,
    bool IsCancelled);

public sealed record RefundPaymentRequest(
    PaymentProvider Provider,
    string MerchantOrderReference,
    string? ExternalPaymentId,
    decimal Amount,
    string Currency);

public sealed record RefundPaymentResult(
    bool Success,
    string? ExternalStatus,
    string? ErrorCode,
    string? ErrorMessage);

public sealed record ParsedPaymentCallback(
    PaymentProvider Provider,
    string MerchantOrderReference,
    string? ExternalPaymentId,
    string? ExternalOrderReference,
    string? ExternalStatus,
    bool IsAuthorized,
    bool IsPaid,
    bool IsFailed,
    bool IsCancelled,
    string? FailureCode,
    string? FailureMessage);*/