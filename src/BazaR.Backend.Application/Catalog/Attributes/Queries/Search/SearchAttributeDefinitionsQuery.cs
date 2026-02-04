using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.Search;

public sealed record SearchAttributeDefinitionsQuery(string Term, int Limit = 20)
    : IRequest<Result<IReadOnlyList<AttributeDefinitionListItemDto>>>;
