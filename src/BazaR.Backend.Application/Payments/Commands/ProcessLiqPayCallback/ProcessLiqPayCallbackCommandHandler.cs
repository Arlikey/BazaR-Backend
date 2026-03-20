using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Repositories;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.ProcessLiqPayCallback;

public sealed class ProcessLiqPayCallbackCommandHandler
    : IRequestHandler<ProcessLiqPayCallbackCommand, Result>
{
    private readonly IPaymentRepository _payments;
    private readonly IPaymentProfileRepository _paymentProfiles;
    private readonly IOrderRepository _orders;
    private readonly ILiqPayCheckoutService _liqPay;
    private readonly IUnitOfWork _uow;

    public ProcessLiqPayCallbackCommandHandler(
        IPaymentRepository payments,
        IPaymentProfileRepository paymentProfiles,
        IOrderRepository orders,
        ILiqPayCheckoutService liqPay,
        IUnitOfWork uow)
    {
        _payments = payments;
        _paymentProfiles = paymentProfiles;
        _orders = orders;
        _liqPay = liqPay;
        _uow = uow;
    }

    public async Task<Result> Handle(ProcessLiqPayCallbackCommand request, CancellationToken ct)
    {
        var merchantOrderReference = LiqPayCallbackRawDecoder.TryExtractMerchantOrderReference(request.Data);
        if (string.IsNullOrWhiteSpace(merchantOrderReference))
        {
            return Result.Failure(new Error(
                "Payment.Callback.Invalid",
                "Merchant order reference could not be extracted from LiqPay callback."));
        }

        var payment = await _payments.GetByMerchantOrderReferenceAsync(merchantOrderReference!, ct);
        if (payment is null)
        {
            return Result.Failure(new Error(
                "Payment.NotFound",
                "Payment was not found."));
        }

        if (payment.Provider != Domain.Payments.PaymentProvider.LiqPay)
        {
            return Result.Failure(new Error(
                "Payment.Provider.Invalid",
                "Payment provider must be LiqPay."));
        }

        var profile = await _paymentProfiles.GetActiveBySellerIdAsync(payment.SellerId, ct);
        if (profile?.LiqPaySettings is null)
        {
            return Result.Failure(new Error(
                "PaymentProfile.LiqPay.Required",
                "LiqPay settings are required."));
        }

        var parsed = _liqPay.ParseAndValidateCallback(
            request.Data,
            request.Signature,
            profile.LiqPaySettings.PrivateKey);

        if (!parsed.IsValid)
        {
            return Result.Failure(new Error(
                "Payment.Callback.InvalidSignature",
                "Invalid LiqPay callback signature."));
        }

        var status = NormalizeStatus(parsed.ExternalStatus);
        Result result;

        if (IsPaidStatus(status))
        {
            result = payment.MarkLiqPayPaid(
                parsed.ExternalPaymentId,
                parsed.ExternalTransactionId,
                parsed.ActualPayType,
                parsed.ProviderAmount,
                parsed.ProviderCurrency,
                parsed.CardMask,
                parsed.CardBank,
                parsed.CardType,
                parsed.ExternalStatus,
                parsed.RawData,
                parsed.RawSignature);

            if (result.IsFailure)
                return result;

            var order = await _orders.GetByIdAsync(payment.OrderId, ct);
            if (order is null)
            {
                return Result.Failure(new Error(
                    "Order.NotFound",
                    "Order was not found."));
            }

            var payOrderResult = order.Pay();
            if (payOrderResult.IsFailure)
                return payOrderResult;

            _orders.Update(order);
        }
        else if (IsAuthorizedStatus(status))
        {
            result = payment.MarkLiqPayAuthorized(
                parsed.ExternalPaymentId,
                parsed.ExternalTransactionId,
                parsed.ActualPayType,
                parsed.ProviderAmount,
                parsed.ProviderCurrency,
                parsed.CardMask,
                parsed.CardBank,
                parsed.CardType,
                parsed.ExternalStatus,
                parsed.RawData,
                parsed.RawSignature);

            if (result.IsFailure)
                return result;
        }
        else
        {
            result = payment.MarkLiqPayFailed(
                parsed.FailureCode,
                parsed.FailureMessage,
                parsed.ExternalPaymentId,
                parsed.ExternalTransactionId,
                parsed.ActualPayType,
                parsed.ProviderAmount,
                parsed.ProviderCurrency,
                parsed.ExternalStatus,
                parsed.RawData,
                parsed.RawSignature);

            if (result.IsFailure)
                return result;
        }

        _payments.Update(payment);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private static string NormalizeStatus(string? status)
        => (status ?? string.Empty).Trim().ToLowerInvariant();

    private static bool IsPaidStatus(string status)
        => status is "success" or "subscribed";

    private static bool IsAuthorizedStatus(string status)
        => status is "wait_accept" or "hold_wait";
}