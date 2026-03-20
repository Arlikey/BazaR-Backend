using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentsByUser;

public sealed class GetPaymentsByUserQueryHandler
    : IRequestHandler<GetPaymentsByUserQuery, Result<IReadOnlyCollection<PaymentDto>>>
{
    private readonly IPaymentRepository _payments;

    public GetPaymentsByUserQueryHandler(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task<Result<IReadOnlyCollection<PaymentDto>>> Handle(GetPaymentsByUserQuery request, CancellationToken ct)
    {
        var payments = await _payments.GetByUserIdAsync(new UserId(request.UserId), ct);
        return Result<IReadOnlyCollection<PaymentDto>>.Success(
            payments.Select(x => x.ToDto()).ToList());
    }
}