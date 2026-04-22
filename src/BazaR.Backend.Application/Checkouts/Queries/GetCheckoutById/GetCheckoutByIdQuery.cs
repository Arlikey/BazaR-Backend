using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed record GetCheckoutByIdQuery(Guid CheckoutId)
    : IRequest<Result<CheckoutDetailsDto>>;