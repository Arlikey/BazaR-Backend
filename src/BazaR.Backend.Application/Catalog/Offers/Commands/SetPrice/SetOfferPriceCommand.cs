using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.SetPrice;

public sealed record SetOfferPriceCommand(
    Guid OfferId,
    decimal Amount,
    string Currency
) : IRequest<Result>;