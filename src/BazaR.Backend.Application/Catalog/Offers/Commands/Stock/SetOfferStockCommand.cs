using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Stock;

public sealed record SetOfferStockCommand(Guid OfferId, int Stock) : IRequest<Result>;