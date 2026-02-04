using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.List;

public sealed record ListAttributeDefinitionsQuery()
    : IRequest<Result<IReadOnlyList<AttributeDefinitionListItemDto>>>;
