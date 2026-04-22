using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed record GetCheckoutLineQuery(
    Guid CheckoutId,
    Guid LineId)
    : IRequest<Result<CheckoutLineDto>>;