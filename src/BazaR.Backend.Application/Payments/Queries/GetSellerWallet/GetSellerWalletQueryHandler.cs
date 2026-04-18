using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetSellerWallet;

public sealed class GetSellerWalletQueryHandler
    : IRequestHandler<GetSellerWalletQuery, Result<SellerWalletDto>>
{
    private readonly IPaymentRepository _payments;

    public GetSellerWalletQueryHandler(IPaymentRepository payments)
    {
        _payments = payments;
    }

    public async Task<Result<SellerWalletDto>> Handle(GetSellerWalletQuery request, CancellationToken ct)
    {
        var sellerId = new SellerId(request.SellerId);

        var balance = await _payments.GetSellerBalanceAsync(sellerId, ct);

        return Result<SellerWalletDto>.Success(new SellerWalletDto
        {
            Amount = balance.Amount,
            Currency = balance.Currency
        });
    }
}