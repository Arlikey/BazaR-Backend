using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Checkouts.DTOs;

using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Shipping;
using BazaR.Backend.Domain.Shippings;

namespace BazaR.Backend.Application.Checkouts.Services;

public sealed class CheckoutSubmissionService : ICheckoutSubmissionService
{
    private readonly IOrderRepository _orders;
    private readonly IShippingRepository _shippings;
    private readonly IPaymentRepository _payments;
    private readonly IOrderFactory _orderFactory;

    public CheckoutSubmissionService(
        IOrderRepository orders,
        IShippingRepository shippings,
        IPaymentRepository payments,
        IOrderFactory orderFactory)
    {
        _orders = orders;
        _shippings = shippings;
        _payments = payments;
        _orderFactory = orderFactory;
    }

    public async Task<Result<CheckoutSubmissionResult>> SubmitAsync(
        Checkout checkout,
        DateTimeOffset nowUtc,
        CancellationToken ct)
    {
        if (checkout is null)
            return Result<CheckoutSubmissionResult>.Failure(
                new Error("Checkout.Required", "Checkout is required."));

        if (!checkout.CanSubmit())
            return Result<CheckoutSubmissionResult>.Failure(
                new Error("Checkout.NotReady", "Checkout is not ready for submission."));

        var orderIds = new List<Guid>();
        var shippingIds = new List<Guid>();
        var paymentIds = new List<Guid>();

        foreach (var line in checkout.Lines)
        {
            if (line.Recipient is null)
            {
                return Result<CheckoutSubmissionResult>.Failure(
                    new Error("Checkout.Line.Recipient.Required", $"Recipient is required for line '{line.Id.Value}'."));
            }

            if (line.Shipping is null)
            {
                return Result<CheckoutSubmissionResult>.Failure(
                    new Error("Checkout.Line.Shipping.Required", $"Shipping is required for line '{line.Id.Value}'."));
            }

            if (line.Payment is null)
            {
                return Result<CheckoutSubmissionResult>.Failure(
                    new Error("Checkout.Line.Payment.Required", $"Payment is required for line '{line.Id.Value}'."));
            }

            var orderResult = _orderFactory.CreateFromCheckoutLine(
                checkout,
                line,
                checkout.UserId,
                nowUtc);

            if (orderResult.IsFailure)
                return Result<CheckoutSubmissionResult>.Failure(orderResult.Error);

            var order = orderResult.Value!;
            _orders.Add(order);
            orderIds.Add(order.Id.Value);

            var shippingResult = CreateShippingFromLine(
                checkout,
                line,
                order.Id,
                nowUtc);

            if (shippingResult.IsFailure)
                return Result<CheckoutSubmissionResult>.Failure(shippingResult.Error);

            var shipping = shippingResult.Value!;
            _shippings.Add(shipping);
            shippingIds.Add(shipping.Id.Value);

            var paymentResult = CreatePaymentFromLine(
                checkout,
                line,
                order.Id,
                order,
                nowUtc);

            if (paymentResult.IsFailure)
                return Result<CheckoutSubmissionResult>.Failure(paymentResult.Error);

            var payment = paymentResult.Value!;
            _payments.Add(payment);
            paymentIds.Add(payment.Id.Value);

            await Task.CompletedTask;
        }

        var result = new CheckoutSubmissionResult(
            orderIds,
            shippingIds,
            paymentIds);

        return Result<CheckoutSubmissionResult>.Success(result);
    }

    private static Result<Shipping> CreateShippingFromLine(
        Checkout checkout,
        CheckoutLine line,
        OrderId orderId,
        DateTimeOffset nowUtc)
    {
        var recipientResult = ShippingRecipient.Create(
            line.Recipient!.FirstName,
            line.Recipient.LastName,
            line.Recipient.Phone,
            line.Recipient.Email);

        if (recipientResult.IsFailure)
            return Result<Shipping>.Failure(recipientResult.Error);

        var shippingSelection = line.Shipping!;

        var destinationResult = ShippingDestination.Create(
            shippingSelection.Country,
            shippingSelection.Region,
            shippingSelection.City,
            shippingSelection.Street,
            shippingSelection.House,
            shippingSelection.Apartment,
            shippingSelection.PostalCode,
            shippingSelection.PickupPointCode,
            shippingSelection.PickupPointName);

        if (destinationResult.IsFailure)
            return Result<Shipping>.Failure(destinationResult.Error);

        var cashOnDeliveryAllowed =
            line.Payment!.Method == PaymentMethod.CashOnDelivery;

        var shippingResult = Shipping.Create(
            ShippingId.New(),
            orderId,
            checkout.UserId,
            line.SellerId,
            shippingSelection.MethodType,
            recipientResult.Value!,
            destinationResult.Value!,
            shippingSelection.Cost,
            cashOnDeliveryAllowed,
            shippingSelection.Comment,
            nowUtc);

        if (shippingResult.IsFailure)
            return Result<Shipping>.Failure(shippingResult.Error);

        return Result<Shipping>.Success(shippingResult.Value!);
    }

    private static Result<Payment> CreatePaymentFromLine(
        Checkout checkout,
        CheckoutLine line,
        OrderId orderId,
        Order order,
        DateTimeOffset nowUtc)
    {
        var merchantOrderReference = ResolveMerchantOrderReference(order);

        /*var paymentResult = Payment.Create(
            PaymentId.New(),
            orderId,
            checkout.UserId,
            line.Payment!.Provider is null
                ? PaymentProvider.Unknown
                : Enum.TryParse<PaymentProvider>(line.Payment.Provider, ignoreCase: true, out var provider)
                    ? provider
                    : PaymentProvider.Unknown,
            line.Payment.Method,
            ResolvePaymentAmount(order, line),
            merchantOrderReference,
            nowUtc);*/

        var paymentResult = Payment.Create(
            PaymentId.New(),
            orderId,
            line.SellerId,  
            checkout.UserId,
            line.Payment!.Provider is null
                ? PaymentProvider.Unknown
                : Enum.TryParse<PaymentProvider>(line.Payment.Provider, ignoreCase: true, out var provider)
                    ? provider
                    : PaymentProvider.Unknown,
            line.Payment.Method,
            ResolvePaymentAmount(order, line),
            merchantOrderReference,
            nowUtc);

        if (paymentResult.IsFailure)
            return Result<Payment>.Failure(paymentResult.Error);

        return Result<Payment>.Success(paymentResult.Value!);
    }

    private static Money ResolvePaymentAmount(
        Order order,
        CheckoutLine line)
    {
        // Если у твоего Order уже есть TotalAmount/Money — замени на него.
        // Пока безопасный fallback: цена линии + стоимость доставки.
        return line.LineTotal + line.Shipping!.Cost;
    }

    private static string ResolveMerchantOrderReference(
        Order order)
    {
        // Подстрой под свой Order:
        // например order.Number.Value или order.Id.Value.ToString()
        return order.Id.Value.ToString();
    }
}