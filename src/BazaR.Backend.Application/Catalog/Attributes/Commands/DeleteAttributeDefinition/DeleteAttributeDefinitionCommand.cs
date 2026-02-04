using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.DeleteAttributeDefinition;

public sealed record DeleteAttributeDefinitionCommand(
    AttributeId AttributeId
) : IRequest<Result>;
