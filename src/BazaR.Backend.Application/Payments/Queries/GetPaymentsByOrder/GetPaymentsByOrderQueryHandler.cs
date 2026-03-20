using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Orders;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentsByOrder;

public sealed class GetPaymentsByOrderQueryHandler
    : IRequestHandler<GetPaymentsByOrderQuery, Result<IReadOnlyCollection<PaymentDto>>>
{
    private readonly IPaymentRepository _payments;

    public GetPaymentsByOrderQueryHandler(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task<Result<IReadOnlyCollection<PaymentDto>>> Handle(GetPaymentsByOrderQuery request, CancellationToken ct)
    {
        var payments = await _payments.GetByOrderIdAsync(new OrderId(request.OrderId), ct);
        return Result<IReadOnlyCollection<PaymentDto>>.Success(
            payments.Select(x => x.ToDto()).ToList());
    }
}