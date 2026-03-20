using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Payments;

namespace BazaR.Backend.Application.Payments;

public static class PaymentMappings
{
    public static PaymentDto ToDto(this Payment payment)
    {
        return new PaymentDto(
            payment.Id.Value,
            payment.OrderId.Value,
            payment.SellerId.Value,
            payment.UserId.Value,
            payment.Provider,
            payment.Method,
            payment.Status,
            payment.Amount.Amount,
            payment.Amount.Currency,
            payment.RefundedAmount.Amount,
            payment.MerchantOrderReference,
            payment.ExternalPaymentId,
            payment.ExternalOrderReference,
            payment.ExternalSessionId,
            payment.ExternalTransactionId,
            payment.ExternalStatus,
            payment.CheckoutActionUrl,
            payment.CheckoutData,
            payment.CheckoutSignature,
            payment.RequestedLiqPayPayType,
            payment.ActualLiqPayPayType,
            payment.CallbackData,
            payment.CallbackSignature,
            payment.CardMask,
            payment.CardBank,
            payment.CardType,
            payment.ProviderAmount,
            payment.ProviderCurrency,
            payment.FailureCode,
            payment.FailureMessage,
            payment.CreatedAtUtc,
            payment.UpdatedAtUtc,
            payment.CheckoutStartedAtUtc,
            payment.CallbackReceivedAtUtc,
            payment.AuthorizedAtUtc,
            payment.PaidAtUtc,
            payment.FailedAtUtc,
            payment.CancelledAtUtc,
            payment.RefundedAtUtc,
            payment.LastProviderSyncAtUtc);
    }
}