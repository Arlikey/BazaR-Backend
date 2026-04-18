using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetSellerWallet;

public sealed record GetSellerWalletQuery(Guid SellerId) : IRequest<Result<SellerWalletDto>>;