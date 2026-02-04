using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.Tree;

public sealed class GetCategoriesTreeQueryHandler
    : IRequestHandler<GetCategoriesTreeQuery, Result<IReadOnlyList<CategoryTreeNodeDto>>>
{
    private readonly ICategoryReadRepository _read;

    public GetCategoriesTreeQueryHandler(ICategoryReadRepository read) => _read = read;

    public async Task<Result<IReadOnlyList<CategoryTreeNodeDto>>> Handle(GetCategoriesTreeQuery request, CancellationToken ct)
        => Result<IReadOnlyList<CategoryTreeNodeDto>>.Success(await _read.GetTreeAsync(ct));
}
