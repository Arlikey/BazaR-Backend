using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentsBySeller;

public sealed class GetPaymentsBySellerQueryHandler
    : IRequestHandler<GetPaymentsBySellerQuery, Result<IReadOnlyCollection<PaymentDto>>>
{
    private readonly IPaymentRepository _payments;

    public GetPaymentsBySellerQueryHandler(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task<Result<IReadOnlyCollection<PaymentDto>>> Handle(GetPaymentsBySellerQuery request, CancellationToken ct)
    {
        var payments = await _payments.GetBySellerIdAsync(new SellerId(request.SellerId), ct);
        return Result<IReadOnlyCollection<PaymentDto>>.Success(
            payments.Select(x => x.ToDto()).ToList());
    }
}