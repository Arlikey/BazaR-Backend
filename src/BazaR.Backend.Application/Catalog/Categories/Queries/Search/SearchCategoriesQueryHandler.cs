using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.Search;

public sealed class SearchCategoriesQueryHandler
    : IRequestHandler<SearchCategoriesQuery, Result<IReadOnlyList<CategoryListItemDto>>>
{
    private readonly ICategoryReadRepository _read;

    public SearchCategoriesQueryHandler(ICategoryReadRepository read) => _read = read;

    public async Task<Result<IReadOnlyList<CategoryListItemDto>>> Handle(SearchCategoriesQuery request, CancellationToken ct)
    {
        var term = request.Term?.Trim() ?? string.Empty;

        var limit = request.Limit;
        if (limit <= 0) limit = 10;
        if (limit > 50) limit = 50;

        return Result<IReadOnlyList<CategoryListItemDto>>.Success(
            await _read.SearchAsync(term, limit, ct));
    }
}
