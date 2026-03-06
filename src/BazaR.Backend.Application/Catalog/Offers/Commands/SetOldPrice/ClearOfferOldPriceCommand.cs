using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.SetOldPrice;

public sealed record ClearOfferOldPriceCommand(Guid OfferId) : IRequest<Result>;