using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Queries.GetCheckoutById;

public sealed record GetCheckoutByIdQuery(Guid CheckoutId) : IRequest<Result<CheckoutVm>>;