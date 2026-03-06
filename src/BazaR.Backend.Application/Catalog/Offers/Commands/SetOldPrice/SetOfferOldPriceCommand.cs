using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.SetOldPrice;

public sealed record SetOfferOldPriceCommand(
    Guid OfferId,
    decimal Amount,
    string? Currency
) : IRequest<Result>;