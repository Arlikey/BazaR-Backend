using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sales.Offers.Commands.SetMyOfferActive;

public sealed record SetMyOfferActiveCommand(
    Guid ProductId,
    bool IsActive
) : IRequest<Result>;
