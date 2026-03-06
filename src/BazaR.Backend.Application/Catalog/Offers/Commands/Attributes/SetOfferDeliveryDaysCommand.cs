using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Attributes;

public sealed record SetOfferDeliveryDaysCommand(Guid OfferId, int? DeliveryDays) : IRequest<Result>;