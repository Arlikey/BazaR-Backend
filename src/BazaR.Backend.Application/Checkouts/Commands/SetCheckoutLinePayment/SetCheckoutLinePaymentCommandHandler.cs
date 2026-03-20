using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.PaymentProfiles;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLinePayment;

public sealed class SetCheckoutLinePaymentCommandHandler
    : IRequestHandler<SetCheckoutLinePaymentCommand, Result>
{
    private readonly ICheckoutRepository _checkouts;
    private readonly IShippingProfileRepository _shippingProfiles;
    private readonly IPaymentProfileRepository _paymentProfiles;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetCheckoutLinePaymentCommandHandler(
        ICheckoutRepository checkouts,
        IShippingProfileRepository shippingProfiles,
        IPaymentProfileRepository paymentProfiles,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _checkouts = checkouts;
        _shippingProfiles = shippingProfiles;
        _paymentProfiles = paymentProfiles;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SetCheckoutLinePaymentCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var checkout = await _checkouts.GetByIdAsync(new CheckoutId(request.CheckoutId), ct);
        if (checkout is null)
            return Result.Failure(new Error("Checkout.NotFound", "Checkout was not found."));

        if (checkout.UserId.Value != _current.UserId)
            return Result.Failure(new Error("Checkout.Forbidden", "You do not own this checkout."));

        var lineId = new CheckoutLineId(request.LineId);
        var line = checkout.Lines.FirstOrDefault(x => x.Id == lineId);
        if (line is null)
            return Result.Failure(new Error("Checkout.Line.NotFound", "Checkout line was not found."));

        if (line.Recipient is null)
            return Result.Failure(new Error("Checkout.Line.Recipient.Required", "Recipient must be set before payment."));

        if (line.Shipping is null)
            return Result.Failure(new Error("Checkout.Line.Shipping.Required", "Shipping must be set before payment."));

        var paymentProfile = await _paymentProfiles.GetActiveBySellerIdAsync(line.SellerId, ct);
        if (paymentProfile is null)
            return Result.Failure(new Error("PaymentProfile.NotFound", "Active payment profile for seller was not found."));

        var paymentMethodType = MapToPaymentProfileMethodType(request.Method, request.Provider);
        if (paymentMethodType == PaymentMethodType.Unknown)
            return Result.Failure(new Error("Payment.Method.Unsupported", "Selected payment method is not supported."));

        var paymentMethod = paymentProfile.FindEnabledMethod(paymentMethodType);
        if (paymentMethod is null)
            return Result.Failure(new Error("PaymentProfile.Method.NotFound", "Selected payment method is not available."));

        if (paymentMethod.MinAmount.HasValue && line.LineTotal.Amount < paymentMethod.MinAmount.Value)
            return Result.Failure(new Error("Payment.Amount.TooSmall", "Order amount is below minimum allowed for selected payment method."));

        if (paymentMethod.MaxAmount.HasValue && line.LineTotal.Amount > paymentMethod.MaxAmount.Value)
            return Result.Failure(new Error("Payment.Amount.TooLarge", "Order amount exceeds maximum allowed for selected payment method."));

        if (paymentMethod.RequiresLiqPay && paymentProfile.LiqPaySettings is null)
            return Result.Failure(new Error("PaymentProfile.LiqPay.Required", "LiqPay settings are required for selected payment method."));

        if (paymentMethod.RequiresBankAccount && paymentProfile.BankAccount is null)
            return Result.Failure(new Error("PaymentProfile.BankAccount.Required", "Bank account is required for selected payment method."));

        if (request.Method == PaymentMethod.CashOnDelivery)
        {
            var shippingProfile = await _shippingProfiles.GetActiveBySellerIdAsync(line.SellerId, ct);
            if (shippingProfile is null)
                return Result.Failure(new Error("ShippingProfile.NotFound", "Active shipping profile for seller was not found."));

            var shippingMethod = shippingProfile.FindEnabledMethod(line.Shipping.MethodType);
            if (shippingMethod is null)
                return Result.Failure(new Error("ShippingProfile.Method.NotFound", "Selected shipping method is not available."));

            if (!shippingMethod.AllowCashOnDelivery)
                return Result.Failure(new Error("Payment.CashOnDelivery.NotAllowed", "Cash on delivery is not allowed for selected shipping method."));
        }

        var selection = PaymentSelection.Create(
            request.Method,
            request.Provider.ToString(),
            request.RequiresOnlineAuthorization);

        checkout.SetLinePayment(
            lineId,
            selection,
            DateTime.UtcNow);

        _checkouts.Update(checkout);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }

    private static PaymentMethodType MapToPaymentProfileMethodType(
        PaymentMethod method,
        PaymentProvider provider)
    {
        if (method == PaymentMethod.CashOnDelivery)
            return PaymentMethodType.CashOnDelivery;

        if (provider == PaymentProvider.LiqPay && method == PaymentMethod.Card)
            return PaymentMethodType.LiqPayCheckout;

        if (method == PaymentMethod.BankTransfer)
            return PaymentMethodType.BankTransferIndividual;

        return PaymentMethodType.Unknown;
    }
}