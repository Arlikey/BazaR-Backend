using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Queries.GetMyActiveCart;

public sealed record GetMyActiveCartQuery()
    : IRequest<Result<CustomerCartDto?>>;