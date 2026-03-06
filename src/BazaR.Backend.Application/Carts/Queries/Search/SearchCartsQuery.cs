using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Queries.Search;

public sealed record SearchCartsQuery(
    CartFilter Filter,
    Pagination Pagination
) : IRequest<Result<CartPagedResult<CartAdminListItemReadModel>>>;