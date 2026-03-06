using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Stock;

public sealed record DecreaseOfferStockCommand(Guid OfferId, int Amount) : IRequest<Result>;