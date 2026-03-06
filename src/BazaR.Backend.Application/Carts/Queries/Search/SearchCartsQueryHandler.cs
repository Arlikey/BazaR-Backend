using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Queries.Search;

public sealed class SearchCartsQueryHandler
    : IRequestHandler<SearchCartsQuery, Result<CartPagedResult<CartAdminListItemReadModel>>>
{
    private readonly ICartReadRepository _carts;

    public SearchCartsQueryHandler(ICartReadRepository carts)
    {
        _carts = carts;
    }

    public async Task<Result<CartPagedResult<CartAdminListItemReadModel>>> Handle(
        SearchCartsQuery request,
        CancellationToken ct)
    {
        // Приводим параметры пагинации к корректным значениям
        var page = request.Pagination.Page < 1 ? 1 : request.Pagination.Page;
        var pageSize = request.Pagination.PageSize is < 1 or > 200
            ? 50
            : request.Pagination.PageSize;

        var pagination = new Pagination(page, pageSize);

        // Выполняем поиск в read-репозитории
        var result = await _carts.SearchAsync(request.Filter, pagination, ct);

        return Result<CartPagedResult<CartAdminListItemReadModel>>.Success(result);
    }
}