using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Queries.List;

public sealed record ListSellersQuery(
    string? Status,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<ListSellersResponse>>;

public sealed record ListSellersResponse(
    IReadOnlyList<SellerListItemDto> Items,
    int Page,
    int PageSize,
    long Total
);
                    