using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.List;

public sealed class ListProductsQueryHandler
    : IRequestHandler<ListProductsQuery, Result<IReadOnlyList<ProductListItemDto>>>
{
    private readonly IProductReadRepository _read;

    public ListProductsQueryHandler(IProductReadRepository read) => _read = read;

    public async Task<Result<IReadOnlyList<ProductListItemDto>>> Handle(ListProductsQuery request, CancellationToken ct)
        => Result<IReadOnlyList<ProductListItemDto>>.Success(await _read.ListAsync(ct));
}
