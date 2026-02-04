using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.GetById;

public sealed class GetAttributeDefinitionByIdQueryHandler
    : IRequestHandler<GetAttributeDefinitionByIdQuery, Result<AttributeDefinitionDetailsDto>>
{
    private readonly IAttributeDefinitionReadRepository _read;

    public GetAttributeDefinitionByIdQueryHandler(IAttributeDefinitionReadRepository read)
    {
        _read = read;
    }

    public async Task<Result<AttributeDefinitionDetailsDto>> Handle(
        GetAttributeDefinitionByIdQuery request,
        CancellationToken ct)
    {
        var dto = await _read.GetByIdAsync(request.Id, ct);
        if (dto is null)
            return Result<AttributeDefinitionDetailsDto>.Failure(AttributeErrors.NotFound);

        return Result<AttributeDefinitionDetailsDto>.Success(dto);
    }
}
