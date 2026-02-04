using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.List;

public sealed class ListAttributeDefinitionsQueryHandler
    : IRequestHandler<ListAttributeDefinitionsQuery, Result<IReadOnlyList<AttributeDefinitionListItemDto>>>
{
    private readonly IAttributeDefinitionReadRepository _read;

    public ListAttributeDefinitionsQueryHandler(IAttributeDefinitionReadRepository read)
    {
        _read = read;
    }

    public async Task<Result<IReadOnlyList<AttributeDefinitionListItemDto>>> Handle(
        ListAttributeDefinitionsQuery request,
        CancellationToken ct)
    {
        var items = await _read.ListAsync(ct);
        return Result<IReadOnlyList<AttributeDefinitionListItemDto>>.Success(items);
    }
}
