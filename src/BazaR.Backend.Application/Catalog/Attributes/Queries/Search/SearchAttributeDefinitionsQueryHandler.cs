using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.Search;

public sealed class SearchAttributeDefinitionsQueryHandler
    : IRequestHandler<SearchAttributeDefinitionsQuery, Result<IReadOnlyList<AttributeDefinitionListItemDto>>>
{
    private readonly IAttributeDefinitionReadRepository _read;

    public SearchAttributeDefinitionsQueryHandler(IAttributeDefinitionReadRepository read)
    {
        _read = read;
    }

    public async Task<Result<IReadOnlyList<AttributeDefinitionListItemDto>>> Handle(
        SearchAttributeDefinitionsQuery request,
        CancellationToken ct)
    {
        var term = request.Term?.Trim() ?? string.Empty;

        var limit = request.Limit;
        if (limit <= 0) limit = 10;
        if (limit > 50) limit = 50;

        var items = await _read.SearchAsync(term, limit, ct);
        return Result<IReadOnlyList<AttributeDefinitionListItemDto>>.Success(items);
    }
}
