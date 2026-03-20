using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.CancelCheckout;

public sealed record CancelCheckoutCommand(Guid CheckoutId) : IRequest<Result>;