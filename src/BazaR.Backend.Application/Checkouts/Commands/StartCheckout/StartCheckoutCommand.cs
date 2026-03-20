using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.StartCheckout;

public sealed record StartCheckoutCommand : IRequest<Result<Guid>>;