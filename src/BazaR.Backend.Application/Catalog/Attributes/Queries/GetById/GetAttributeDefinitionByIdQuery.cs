using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Queries.GetById;

public sealed record GetAttributeDefinitionByIdQuery(AttributeId Id)
    : IRequest<Result<AttributeDefinitionDetailsDto>>;
