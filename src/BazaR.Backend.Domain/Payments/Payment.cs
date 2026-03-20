using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Payments;

public sealed class Payment : AggregateRoot<PaymentId>
{
    private Payment() { }

    private Payment(
        PaymentId id,
        OrderId orderId,
        SellerId sellerId,
        UserId userId,
        PaymentProvider provider,
        PaymentMethod method,
        Money amount,
        string merchantOrderReference,
        DateTimeOffset nowUtc) : base(id)
    {
        Id = id;
        OrderId = orderId;
        SellerId = sellerId;
        UserId = userId;
        Provider = provider;
        Method = method;
        Amount = amount;
        RefundedAmount = Money.Zero(amount.Currency);
        MerchantOrderReference = merchantOrderReference.Trim();

        Status = PaymentStatus.Pending;
        CreatedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public OrderId OrderId { get; private set; } = default!;
    public SellerId SellerId { get; private set; } = default!;
    public UserId UserId { get; private set; } = default!;

    public PaymentProvider Provider { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }

    public Money Amount { get; private set; } = default!;
    public Money RefundedAmount { get; private set; } = default!;

    public string MerchantOrderReference { get; private set; } = default!;

    public string? ExternalPaymentId { get; private set; }
    public string? ExternalOrderReference { get; private set; }
    public string? ExternalSessionId { get; private set; }
    public string? ExternalTransactionId { get; private set; }
    public string? ExternalStatus { get; private set; }

    public string? CheckoutActionUrl { get; private set; }
    public string? CheckoutData { get; private set; }
    public string? CheckoutSignature { get; private set; }

    public LiqPayPayType? RequestedLiqPayPayType { get; private set; }
    public LiqPayPayType? ActualLiqPayPayType { get; private set; }

    public string? CallbackData { get; private set; }
    public string? CallbackSignature { get; private set; }

    public string? CardMask { get; private set; }
    public string? CardBank { get; private set; }
    public string? CardType { get; private set; }

    public decimal? ProviderAmount { get; private set; }
    public string? ProviderCurrency { get; private set; }

    public string? FailureCode { get; private set; }
    public string? FailureMessage { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public DateTimeOffset? CheckoutStartedAtUtc { get; private set; }
    public DateTimeOffset? CallbackReceivedAtUtc { get; private set; }
    public DateTimeOffset? AuthorizedAtUtc { get; private set; }
    public DateTimeOffset? PaidAtUtc { get; private set; }
    public DateTimeOffset? FailedAtUtc { get; private set; }
    public DateTimeOffset? CancelledAtUtc { get; private set; }
    public DateTimeOffset? RefundedAtUtc { get; private set; }
    public DateTimeOffset? LastProviderSyncAtUtc { get; private set; }

    public bool IsPaid => Status == PaymentStatus.Paid;
    public bool IsFinal => Status is PaymentStatus.Cancelled or PaymentStatus.Refunded;

    public static Result<Payment> Create(
        PaymentId id,
        OrderId orderId,
        SellerId sellerId,
        UserId userId,
        PaymentProvider provider,
        PaymentMethod method,
        Money amount,
        string merchantOrderReference,
        DateTimeOffset? nowUtc = null)
    {
        if (id.Value == Guid.Empty)
            return Result<Payment>.Failure(PaymentErrors.PaymentIdRequired);

        if (orderId.Value == Guid.Empty)
            return Result<Payment>.Failure(PaymentErrors.OrderIdRequired);

        if (sellerId.Value == Guid.Empty)
            return Result<Payment>.Failure(new Error("Payment.SellerId.Required", "Seller id is required."));

        if (userId.Value == Guid.Empty)
            return Result<Payment>.Failure(PaymentErrors.UserIdRequired);

        if (amount is null)
            return Result<Payment>.Failure(PaymentErrors.AmountRequired);

        if (amount.Amount <= 0)
            return Result<Payment>.Failure(PaymentErrors.AmountMustBePositive);

        if (string.IsNullOrWhiteSpace(merchantOrderReference))
            return Result<Payment>.Failure(PaymentErrors.MerchantOrderReferenceRequired);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        var payment = new Payment(
            id,
            orderId,
            sellerId,
            userId,
            provider,
            method,
            amount,
            merchantOrderReference,
            now);

        payment.AddDomainEvent(new PaymentCreatedDomainEvent(payment.Id, payment.OrderId));
        return Result<Payment>.Success(payment);
    }

    public Result AttachCheckout(
        string checkoutActionUrl,
        string data,
        string signature,
        string? externalOrderReference,
        string? externalSessionId,
        string? externalStatus,
        DateTimeOffset? nowUtc = null)
    {
        if (Method == PaymentMethod.CashOnDelivery)
            return Result.Failure(PaymentErrors.CashOnDeliveryDoesNotRequireCheckout);

        if (Method == PaymentMethod.BankTransfer)
            return Result.Failure(PaymentErrors.BankTransferDoesNotRequireCheckout);

        if (Status is not PaymentStatus.Pending and not PaymentStatus.RequiresAction)
            return Result.Failure(PaymentErrors.CheckoutCannotBeAttached(Status));

        if (string.IsNullOrWhiteSpace(checkoutActionUrl))
            return Result.Failure(PaymentErrors.CheckoutActionUrlRequired);

        if (string.IsNullOrWhiteSpace(data))
            return Result.Failure(PaymentErrors.CheckoutDataRequired);

        if (string.IsNullOrWhiteSpace(signature))
            return Result.Failure(PaymentErrors.CheckoutSignatureRequired);

        CheckoutActionUrl = checkoutActionUrl.Trim();
        CheckoutData = data.Trim();
        CheckoutSignature = signature.Trim();

        ExternalOrderReference = NormalizeNullable(externalOrderReference);
        ExternalSessionId = NormalizeNullable(externalSessionId);
        ExternalStatus = NormalizeNullable(externalStatus);

        Status = PaymentStatus.RequiresAction;
        CheckoutStartedAtUtc ??= nowUtc ?? DateTimeOffset.UtcNow;
        Touch(nowUtc);

        AddDomainEvent(new PaymentCheckoutAttachedDomainEvent(Id, OrderId));
        return Result.Success();
    }

    public Result AttachLiqPayCheckout(
        string checkoutActionUrl,
        string data,
        string signature,
        string? externalOrderReference,
        string? externalSessionId,
        LiqPayPayType? requestedPayType,
        string? externalStatus,
        DateTimeOffset? nowUtc = null)
    {
        if (Provider != PaymentProvider.LiqPay)
        {
            return Result.Failure(new Error(
                "Payment.Provider.Invalid",
                "LiqPay checkout can be attached only for LiqPay provider."));
        }

        var result = AttachCheckout(
            checkoutActionUrl,
            data,
            signature,
            externalOrderReference,
            externalSessionId,
            externalStatus,
            nowUtc);

        if (result.IsFailure)
            return result;

        RequestedLiqPayPayType = requestedPayType;
        Touch(nowUtc);

        return Result.Success();
    }

    public Result MarkAuthorized(
        string? externalPaymentId,
        string? externalStatus,
        DateTimeOffset? authorizedAtUtc = null)
    {
        if (Status is PaymentStatus.Cancelled
            or PaymentStatus.Refunded
            or PaymentStatus.PartiallyRefunded
            or PaymentStatus.Paid)
        {
            return Result.Failure(PaymentErrors.InvalidTransition(Status, PaymentStatus.Authorized));
        }

        ExternalPaymentId ??= NormalizeNullable(externalPaymentId);
        ExternalStatus = NormalizeNullable(externalStatus);

        Status = PaymentStatus.Authorized;
        AuthorizedAtUtc ??= authorizedAtUtc ?? DateTimeOffset.UtcNow;
        Touch(authorizedAtUtc);

        AddDomainEvent(new PaymentAuthorizedDomainEvent(Id, OrderId));
        return Result.Success();
    }

    public Result MarkLiqPayAuthorized(
        string? externalPaymentId,
        string? externalTransactionId,
        LiqPayPayType? actualPayType,
        decimal? providerAmount,
        string? providerCurrency,
        string? cardMask,
        string? cardBank,
        string? cardType,
        string? externalStatus,
        string? callbackData,
        string? callbackSignature,
        DateTimeOffset? authorizedAtUtc = null)
    {
        if (Provider != PaymentProvider.LiqPay)
        {
            return Result.Failure(new Error(
                "Payment.Provider.Invalid",
                "LiqPay callback can be applied only for LiqPay provider."));
        }

        RecordProviderSnapshot(
            externalPaymentId,
            externalTransactionId,
            actualPayType,
            providerAmount,
            providerCurrency,
            cardMask,
            cardBank,
            cardType,
            externalStatus,
            callbackData,
            callbackSignature,
            authorizedAtUtc);

        return MarkAuthorized(externalPaymentId, externalStatus, authorizedAtUtc);
    }

    public Result MarkPaid(
        string? externalPaymentId,
        string? externalStatus,
        DateTimeOffset? paidAtUtc = null)
    {
        if (Status == PaymentStatus.Paid)
            return Result.Success();

        if (Status is PaymentStatus.Cancelled
            or PaymentStatus.Refunded
            or PaymentStatus.PartiallyRefunded)
        {
            return Result.Failure(PaymentErrors.InvalidTransition(Status, PaymentStatus.Paid));
        }

        ExternalPaymentId ??= NormalizeNullable(externalPaymentId);
        ExternalStatus = NormalizeNullable(externalStatus);

        Status = PaymentStatus.Paid;
        PaidAtUtc ??= paidAtUtc ?? DateTimeOffset.UtcNow;
        Touch(paidAtUtc);

        AddDomainEvent(new PaymentPaidDomainEvent(Id, OrderId));
        return Result.Success();
    }

    public Result MarkLiqPayPaid(
        string? externalPaymentId,
        string? externalTransactionId,
        LiqPayPayType? actualPayType,
        decimal? providerAmount,
        string? providerCurrency,
        string? cardMask,
        string? cardBank,
        string? cardType,
        string? externalStatus,
        string? callbackData,
        string? callbackSignature,
        DateTimeOffset? paidAtUtc = null)
    {
        if (Provider != PaymentProvider.LiqPay)
        {
            return Result.Failure(new Error(
                "Payment.Provider.Invalid",
                "LiqPay callback can be applied only for LiqPay provider."));
        }

        RecordProviderSnapshot(
            externalPaymentId,
            externalTransactionId,
            actualPayType,
            providerAmount,
            providerCurrency,
            cardMask,
            cardBank,
            cardType,
            externalStatus,
            callbackData,
            callbackSignature,
            paidAtUtc);

        return MarkPaid(externalPaymentId, externalStatus, paidAtUtc);
    }

    public Result MarkFailed(
        string? failureCode,
        string? failureMessage,
        string? externalStatus,
        DateTimeOffset? failedAtUtc = null)
    {
        if (Status is PaymentStatus.Paid
            or PaymentStatus.Cancelled
            or PaymentStatus.Refunded
            or PaymentStatus.PartiallyRefunded)
        {
            return Result.Failure(PaymentErrors.InvalidTransition(Status, PaymentStatus.Failed));
        }

        FailureCode = NormalizeNullable(failureCode);
        FailureMessage = NormalizeNullable(failureMessage);
        ExternalStatus = NormalizeNullable(externalStatus);

        Status = PaymentStatus.Failed;
        FailedAtUtc ??= failedAtUtc ?? DateTimeOffset.UtcNow;
        Touch(failedAtUtc);

        AddDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, FailureCode, FailureMessage));
        return Result.Success();
    }

    public Result MarkLiqPayFailed(
        string? failureCode,
        string? failureMessage,
        string? externalPaymentId,
        string? externalTransactionId,
        LiqPayPayType? actualPayType,
        decimal? providerAmount,
        string? providerCurrency,
        string? externalStatus,
        string? callbackData,
        string? callbackSignature,
        DateTimeOffset? failedAtUtc = null)
    {
        if (Provider != PaymentProvider.LiqPay)
        {
            return Result.Failure(new Error(
                "Payment.Provider.Invalid",
                "LiqPay callback can be applied only for LiqPay provider."));
        }

        RecordProviderSnapshot(
            externalPaymentId,
            externalTransactionId,
            actualPayType,
            providerAmount,
            providerCurrency,
            null,
            null,
            null,
            externalStatus,
            callbackData,
            callbackSignature,
            failedAtUtc);

        return MarkFailed(failureCode, failureMessage, externalStatus, failedAtUtc);
    }

    public Result Cancel(string? externalStatus = null, DateTimeOffset? cancelledAtUtc = null)
    {
        if (Status == PaymentStatus.Cancelled)
            return Result.Success();

        if (Status is PaymentStatus.Paid
            or PaymentStatus.PartiallyRefunded
            or PaymentStatus.Refunded)
        {
            return Result.Failure(PaymentErrors.InvalidTransition(Status, PaymentStatus.Cancelled));
        }

        ExternalStatus = NormalizeNullable(externalStatus);

        Status = PaymentStatus.Cancelled;
        CancelledAtUtc ??= cancelledAtUtc ?? DateTimeOffset.UtcNow;
        Touch(cancelledAtUtc);

        AddDomainEvent(new PaymentCancelledDomainEvent(Id, OrderId));
        return Result.Success();
    }

    public Result Refund(Money amount, DateTimeOffset? refundedAtUtc = null)
    {
        if (Status is not PaymentStatus.Paid and not PaymentStatus.PartiallyRefunded)
            return Result.Failure(PaymentErrors.OnlyPaidCanBeRefunded);

        if (amount is null)
            return Result.Failure(PaymentErrors.RefundAmountRequired);

        if (amount.Currency != Amount.Currency)
            return Result.Failure(PaymentErrors.CurrencyMismatch);

        if (amount.Amount <= 0)
            return Result.Failure(PaymentErrors.RefundAmountMustBePositive);

        var newRefunded = RefundedAmount + amount;

        if (newRefunded.Amount > Amount.Amount)
            return Result.Failure(PaymentErrors.RefundCannotExceedPaidAmount);

        RefundedAmount = newRefunded;
        RefundedAtUtc = refundedAtUtc ?? DateTimeOffset.UtcNow;
        Touch(refundedAtUtc);

        Status = RefundedAmount.Amount == Amount.Amount
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;

        AddDomainEvent(new PaymentRefundedDomainEvent(Id, OrderId, amount.Amount, RefundedAmount.Amount));
        return Result.Success();
    }

    private void RecordProviderSnapshot(
        string? externalPaymentId,
        string? externalTransactionId,
        LiqPayPayType? actualPayType,
        decimal? providerAmount,
        string? providerCurrency,
        string? cardMask,
        string? cardBank,
        string? cardType,
        string? externalStatus,
        string? callbackData,
        string? callbackSignature,
        DateTimeOffset? nowUtc)
    {
        ExternalPaymentId ??= NormalizeNullable(externalPaymentId);
        ExternalTransactionId ??= NormalizeNullable(externalTransactionId);
        ActualLiqPayPayType ??= actualPayType;

        ProviderAmount ??= providerAmount;
        ProviderCurrency ??= NormalizeNullable(providerCurrency);

        CardMask ??= NormalizeNullable(cardMask);
        CardBank ??= NormalizeNullable(cardBank);
        CardType ??= NormalizeNullable(cardType);

        ExternalStatus = NormalizeNullable(externalStatus);
        CallbackData = NormalizeNullable(callbackData);
        CallbackSignature = NormalizeNullable(callbackSignature);

        CallbackReceivedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;
        LastProviderSyncAtUtc = nowUtc ?? DateTimeOffset.UtcNow;
    }

    private void Touch(DateTimeOffset? nowUtc = null)
        => UpdatedAtUtc = nowUtc ?? DateTimeOffset.UtcNow;

    private static string? NormalizeNullable(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}