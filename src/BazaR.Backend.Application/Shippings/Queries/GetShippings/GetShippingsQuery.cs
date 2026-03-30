using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.GetShippings;

public sealed record GetShippingsQuery(
    string? Query,
    int? Method,
    int? Status,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<ShippingListItemDto>>>;