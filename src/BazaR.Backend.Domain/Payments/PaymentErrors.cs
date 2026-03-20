using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Payments;

public static class PaymentErrors
{
    public static readonly Error PaymentIdRequired =
        new("Payment.Id.Required", "Payment id is required.");

    public static readonly Error OrderIdRequired =
        new("Payment.OrderId.Required", "Order id is required.");

    public static readonly Error UserIdRequired =
        new("Payment.UserId.Required", "User id is required.");

    public static readonly Error AmountRequired =
        new("Payment.Amount.Required", "Payment amount is required.");

    public static readonly Error AmountMustBePositive =
        new("Payment.Amount.MustBePositive", "Payment amount must be positive.");

    public static readonly Error MerchantOrderReferenceRequired =
        new("Payment.MerchantOrderReference.Required", "Merchant order reference is required.");

    public static readonly Error CheckoutActionUrlRequired =
        new("Payment.Checkout.ActionUrl.Required", "Checkout action url is required.");

    public static readonly Error CheckoutDataRequired =
        new("Payment.Checkout.Data.Required", "Checkout data is required.");

    public static readonly Error CheckoutSignatureRequired =
        new("Payment.Checkout.Signature.Required", "Checkout signature is required.");

    public static readonly Error RefundAmountRequired =
        new("Payment.Refund.Amount.Required", "Refund amount is required.");

    public static readonly Error RefundAmountMustBePositive =
        new("Payment.Refund.Amount.MustBePositive", "Refund amount must be positive.");

    public static readonly Error RefundCannotExceedPaidAmount =
        new("Payment.Refund.CannotExceedPaidAmount", "Refund amount cannot exceed paid amount.");

    public static readonly Error OnlyPaidCanBeRefunded =
        new("Payment.Refund.OnlyPaidCanBeRefunded", "Only paid payment can be refunded.");

    public static readonly Error CurrencyMismatch =
        new("Payment.Currency.Mismatch", "Currency mismatch.");

    public static readonly Error CashOnDeliveryDoesNotRequireCheckout =
        new("Payment.Checkout.CashOnDelivery.NotRequired", "Cash on delivery does not require hosted checkout.");

    public static readonly Error BankTransferDoesNotRequireCheckout =
        new("Payment.Checkout.BankTransfer.NotRequired", "Bank transfer does not require hosted checkout.");

    public static Error CheckoutCannotBeAttached(PaymentStatus status) =>
        new("Payment.Checkout.InvalidStatus", $"Checkout cannot be attached when payment is in status '{status}'.");

    public static Error InvalidTransition(PaymentStatus from, PaymentStatus to) =>
        new("Payment.Status.InvalidTransition", $"Invalid payment status transition from '{from}' to '{to}'.");
}