using BazaR.Backend.Application.Catalog.Offers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

public sealed record GetOfferByProductIdQuery(Guid ProductId)
    : IRequest<Result<OfferDetailsDto?>>;