using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.GetOptions;

public sealed class GetAttributeOptionsQueryHandler
    : IRequestHandler<GetAttributeOptionsQuery, Result<IReadOnlyList<AttributeOptionDto>>>
{
    private readonly IAttributeDefinitionReadRepository _read;

    public GetAttributeOptionsQueryHandler(IAttributeDefinitionReadRepository read)
    {
        _read = read;
    }

    public async Task<Result<IReadOnlyList<AttributeOptionDto>>> Handle(
        GetAttributeOptionsQuery request,
        CancellationToken ct)
    {
        

        var options = await _read.GetOptionsAsync(request.AttributeId, ct);
        return Result<IReadOnlyList<AttributeOptionDto>>.Success(options);
    }
}
