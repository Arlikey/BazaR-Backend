using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.RefundPayment;

public sealed class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, Result>
{
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _uow;

    public RefundPaymentCommandHandler(
        IPaymentRepository payments,
        IUnitOfWork uow)
    {
        _payments = payments;
        _uow = uow;
    }

    public async Task<Result> Handle(RefundPaymentCommand request, CancellationToken ct)
    {
        var payment = await _payments.GetByIdAsync(new PaymentId(request.PaymentId), ct);
        if (payment is null)
        {
            return Result.Failure(new Error(
                "Payment.NotFound",
                "Payment was not found."));
        }

        var amountResult = Money.Create(request.Amount, request.Currency);
        if (amountResult.IsFailure)
            return Result.Failure(amountResult.Error);

        var result = payment.Refund(amountResult.Value!);
        if (result.IsFailure)
            return result;

        _payments.Update(payment);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}