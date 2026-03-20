using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Payments;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentById;

public sealed class GetPaymentByIdQueryHandler
    : IRequestHandler<GetPaymentByIdQuery, Result<PaymentDto>>
{
    private readonly IPaymentRepository _payments;

    public GetPaymentByIdQueryHandler(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task<Result<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken ct)
    {
        var payment = await _payments.GetByIdAsync(new PaymentId(request.PaymentId), ct);
        if (payment is null)
        {
            return Result<PaymentDto>.Failure(new Error(
                "Payment.NotFound",
                "Payment was not found."));
        }

        return Result<PaymentDto>.Success(payment.ToDto());
    }
}