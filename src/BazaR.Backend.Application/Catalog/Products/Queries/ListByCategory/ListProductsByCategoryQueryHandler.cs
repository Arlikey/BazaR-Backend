using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.ListByCategory;

public sealed class ListProductsByCategoryQueryHandler
    : IRequestHandler<ListProductsByCategoryQuery, Result<IReadOnlyList<ProductListItemDto>>>
{
    private readonly IProductReadRepository _read;

    public ListProductsByCategoryQueryHandler(IProductReadRepository read) => _read = read;

    public async Task<Result<IReadOnlyList<ProductListItemDto>>> Handle(ListProductsByCategoryQuery request, CancellationToken ct)
        => Result<IReadOnlyList<ProductListItemDto>>.Success(await _read.ListByCategoryAsync(request.CategoryId, ct));
}
