using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Queries.GetById;

public sealed record GetCartByIdQuery(Guid CartId)
    : IRequest<Result<CartReadModel?>>;