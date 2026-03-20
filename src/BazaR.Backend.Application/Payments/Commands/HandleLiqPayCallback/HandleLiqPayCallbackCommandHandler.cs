/*using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Payments;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Repositories;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.HandleLiqPayCallback;

public sealed class HandleLiqPayCallbackCommandHandler : IRequestHandler<HandleLiqPayCallbackCommand, Result>
{
    private readonly IPaymentGateway _gateway;
    private readonly IPaymentRepository _payments;
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;

    public HandleLiqPayCallbackCommandHandler(
        IPaymentGateway gateway,
        IPaymentRepository payments,
        IOrderRepository orders,
        IUnitOfWork uow)
    {
        _gateway = gateway;
        _payments = payments;
        _orders = orders;
        _uow = uow;
    }

    public async Task<Result> Handle(HandleLiqPayCallbackCommand request, CancellationToken ct)
    {
        var parsed = await _gateway.ParseCallbackAsync(request.Payload, request.Headers, ct);

        var payment = await _payments.GetByMerchantOrderReferenceAsync(parsed.MerchantOrderReference, ct);
        if (payment is null)
            return Result.Failure(new Error("Payment.NotFound", "Payment not found."));

        Result paymentResult;

        if (parsed.IsPaid)
        {
            paymentResult = payment.MarkPaid(parsed.ExternalPaymentId, parsed.ExternalStatus);
            if (paymentResult.IsFailure) return paymentResult;

            var order = await _orders.GetByIdAsync(payment.OrderId, ct);
            if (order is not null)
            {
                // Подстрой под свой Order агрегат:
                // var res = order.MarkPaid();
                // if (res.IsFailure) return res;
            }
        }
        else if (parsed.IsAuthorized)
        {
            paymentResult = payment.MarkAuthorized(parsed.ExternalPaymentId, parsed.ExternalStatus);
            if (paymentResult.IsFailure) return paymentResult;
        }
        else if (parsed.IsCancelled)
        {
            paymentResult = payment.Cancel(parsed.ExternalStatus);
            if (paymentResult.IsFailure) return paymentResult;
        }
        else if (parsed.IsFailed)
        {
            paymentResult = payment.MarkFailed(
                parsed.FailureCode,
                parsed.FailureMessage,
                parsed.ExternalStatus);

            if (paymentResult.IsFailure) return paymentResult;
        }
        else
        {
            return Result.Success();
        }

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}*/