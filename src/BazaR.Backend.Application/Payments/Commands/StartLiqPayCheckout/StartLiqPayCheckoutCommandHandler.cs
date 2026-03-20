using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.StartLiqPayCheckout;

public sealed class StartLiqPayCheckoutCommandHandler
    : IRequestHandler<StartLiqPayCheckoutCommand, Result<LiqPayCheckoutStartResult>>
{
    private readonly IPaymentRepository _payments;
    private readonly IPaymentProfileRepository _paymentProfiles;
    private readonly ILiqPayCheckoutService _liqPay;
    private readonly IUnitOfWork _uow;

    public StartLiqPayCheckoutCommandHandler(
        IPaymentRepository payments,
        IPaymentProfileRepository paymentProfiles,
        ILiqPayCheckoutService liqPay,
        IUnitOfWork uow)
    {
        _payments = payments;
        _paymentProfiles = paymentProfiles;
        _liqPay = liqPay;
        _uow = uow;
    }

    public async Task<Result<LiqPayCheckoutStartResult>> Handle(StartLiqPayCheckoutCommand request, CancellationToken ct)
    {
        var payment = await _payments.GetByIdAsync(new PaymentId(request.PaymentId), ct);
        if (payment is null)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("Payment.NotFound", "Payment was not found."));
        }

        if (payment.Provider != PaymentProvider.LiqPay)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("Payment.Provider.Invalid", "Payment provider must be LiqPay."));
        }

        var profile = await _paymentProfiles.GetActiveBySellerIdAsync(payment.SellerId, ct);
        if (profile is null)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.NotFound", "Active payment profile for seller was not found."));
        }

        if (profile.LiqPaySettings is null)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.LiqPay.Required", "LiqPay settings are required."));
        }

        var requiredMethodType = MapPayTypeToProfileMethod(request.PayType);

        var method = profile.FindEnabledMethod(requiredMethodType);
        if (method is null)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.Method.NotFound", "Requested payment method is not enabled for seller."));
        }

        if (!method.RequiresLiqPay)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.Method.Invalid", "Selected payment method is not configured for LiqPay."));
        }

        if (!IsPayTypeAllowedBySettings(profile.LiqPaySettings, request.PayType))
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.LiqPay.PayType.NotAllowed", "Selected LiqPay pay type is not allowed."));
        }

        if (method.MinAmount.HasValue && payment.Amount.Amount < method.MinAmount.Value)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("Payment.Amount.TooSmall", "Payment amount is below minimum allowed."));
        }

        if (method.MaxAmount.HasValue && payment.Amount.Amount > method.MaxAmount.Value)
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("Payment.Amount.TooLarge", "Payment amount exceeds maximum allowed."));
        }

        if (string.IsNullOrWhiteSpace(profile.LiqPaySettings.ResultUrl))
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.LiqPay.ResultUrl.Required", "LiqPay result URL is required."));
        }

        if (string.IsNullOrWhiteSpace(profile.LiqPaySettings.ServerCallbackUrl))
        {
            return Result<LiqPayCheckoutStartResult>.Failure(
                new Error("PaymentProfile.LiqPay.ServerUrl.Required", "LiqPay server callback URL is required."));
        }

        var payload = _liqPay.CreateCheckout(new LiqPayCheckoutRequest(
            profile.LiqPaySettings.PublicKey,
            profile.LiqPaySettings.PrivateKey,
            payment.Amount.Amount,
            payment.Amount.Currency,
            request.Description,
            payment.MerchantOrderReference,
            profile.LiqPaySettings.ResultUrl!,
            profile.LiqPaySettings.ServerCallbackUrl!,
            request.PayType));

        var attachResult = payment.AttachLiqPayCheckout(
            payload.ActionUrl,
            payload.Data,
            payload.Signature,
            payload.ExternalOrderReference,
            payload.ExternalSessionId,
            request.PayType,
            payload.ExternalStatus);

        if (attachResult.IsFailure)
            return Result<LiqPayCheckoutStartResult>.Failure(attachResult.Error);

        _payments.Update(payment);
        await _uow.SaveChangesAsync(ct);

        return Result<LiqPayCheckoutStartResult>.Success(new LiqPayCheckoutStartResult(
            payment.Id.Value,
            payload.ActionUrl,
            payload.Data,
            payload.Signature));
    }

    private static PaymentMethodType MapPayTypeToProfileMethod(LiqPayPayType payType)
    {
        return payType switch
        {
            LiqPayPayType.Card => PaymentMethodType.LiqPayCheckout,
            LiqPayPayType.PrivatPay => PaymentMethodType.PrivatPay,
            LiqPayPayType.Installments => PaymentMethodType.Installments,
            _ => PaymentMethodType.Unknown
        };
    }

    private static bool IsPayTypeAllowedBySettings(LiqPaySettings settings, LiqPayPayType payType)
    {
        return payType switch
        {
            LiqPayPayType.Card => settings.CheckoutEnabled,
            LiqPayPayType.PrivatPay => settings.PrivatPayEnabled,
            LiqPayPayType.Installments => settings.InstallmentsEnabled,
            _ => false
        };
    }
}