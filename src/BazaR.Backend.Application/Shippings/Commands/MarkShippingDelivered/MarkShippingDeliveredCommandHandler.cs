using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingDelivered;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Repositories;
using BazaR.Backend.Domain.Shippings;
using MediatR;

public sealed class MarkShippingDeliveredCommandHandler
    : IRequestHandler<MarkShippingDeliveredCommand, Result>
{
    private readonly IShippingRepository _shippings;
    private readonly IOrderRepository _orders;
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _uow;

    public MarkShippingDeliveredCommandHandler(
        IShippingRepository shippings,
        IOrderRepository orders,
        IPaymentRepository payments,
        IUnitOfWork uow)
    {
        _shippings = shippings;
        _orders = orders;
        _payments = payments;
        _uow = uow;
    }

    public async Task<Result> Handle(
        MarkShippingDeliveredCommand request,
        CancellationToken ct)
    {
        var shipping = await _shippings.GetByIdAsync(new ShippingId(request.ShippingId), ct);
        if (shipping is null)
        {
            return Result.Failure(new Error(
                "Shipping.NotFound",
                "Shipping was not found."));
        }

        var shippingResult = shipping.MarkDelivered();
        if (shippingResult.IsFailure)
            return shippingResult;

        var order = await _orders.GetByIdAsync(shipping.OrderId, ct);
        if (order is null)
        {
            return Result.Failure(new Error(
                "Order.NotFound",
                "Order was not found."));
        }

        var orderResult = order.Deliver();
        if (orderResult.IsFailure)
            return orderResult;

        var payment = await _payments.GetSingleByOrderIdAsync(shipping.OrderId, ct);

        if (payment is null)
        {
            return Result.Failure(new Error(
                "Payment.NotFound",
                "Payment for order was not found."));
        }

        if (payment.Method != PaymentMethod.CashOnDelivery)
        {
            return Result.Failure(new Error(
                "Payment.Method.Invalid",
                "Payment method must be cash on delivery."));
        }

        var paymentResult = payment.MarkPaid(null, "cash_on_delivery_paid");
        if (paymentResult.IsFailure)
            return paymentResult;

        _payments.Update(payment);

        _shippings.Update(shipping);
        _orders.Update(order);

        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}