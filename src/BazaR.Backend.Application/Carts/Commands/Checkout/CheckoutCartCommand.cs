using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.Checkout;

public sealed record CheckoutCartCommand() : IRequest<Result<Guid>>;