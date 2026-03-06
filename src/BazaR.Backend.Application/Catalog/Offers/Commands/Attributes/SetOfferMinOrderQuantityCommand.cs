using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Attributes;

public sealed record SetOfferMinOrderQuantityCommand(Guid OfferId, int MinOrderQuantity) : IRequest<Result>;