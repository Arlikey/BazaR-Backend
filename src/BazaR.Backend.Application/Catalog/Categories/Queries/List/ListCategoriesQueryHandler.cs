using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.List;

public sealed class ListCategoriesQueryHandler
    : IRequestHandler<ListCategoriesQuery, Result<IReadOnlyList<CategoryListItemDto>>>
{
    private readonly ICategoryReadRepository _read;

    public ListCategoriesQueryHandler(ICategoryReadRepository read) => _read = read;

    public async Task<Result<IReadOnlyList<CategoryListItemDto>>> Handle(ListCategoriesQuery request, CancellationToken ct)
        => Result<IReadOnlyList<CategoryListItemDto>>.Success(await _read.ListAsync(ct));
}
