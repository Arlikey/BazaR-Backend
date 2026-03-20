using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using BazaR.Backend.Domain.Payments;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler
    : IRequestHandler<CreatePaymentCommand, Result<Guid>>
{
    private readonly IPaymentRepository _payments;
    private readonly IUnitOfWork _uow;

    public CreatePaymentCommandHandler(
        IPaymentRepository payments,
        IUnitOfWork uow)
    {
        _payments = payments;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        var existing = await _payments.GetByMerchantOrderReferenceAsync(request.MerchantOrderReference, ct);
        if (existing is not null)
            return Result<Guid>.Success(existing.Id.Value);

        var amountResult = Money.Create(request.Amount, request.Currency);
        if (amountResult.IsFailure)
            return Result<Guid>.Failure(amountResult.Error);

        var paymentResult = Payment.Create(
            PaymentId.New(),
            new OrderId(request.OrderId),
            new SellerId(request.SellerId),
            new UserId(request.UserId),
            request.Provider,
            request.Method,
            amountResult.Value!,
            request.MerchantOrderReference);

        if (paymentResult.IsFailure)
            return Result<Guid>.Failure(paymentResult.Error);

        var payment = paymentResult.Value!;

        _payments.Add(payment);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(payment.Id.Value);
    }
}