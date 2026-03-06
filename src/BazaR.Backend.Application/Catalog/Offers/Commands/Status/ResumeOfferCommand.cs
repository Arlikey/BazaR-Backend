using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Status;

public sealed record ResumeOfferCommand(Guid OfferId) : IRequest<Result>;