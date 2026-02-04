using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.UpdateAttributeDefinition;

public sealed record UpdateAttributeDefinitionCommand(
    Guid AttributeId,
    string? Name,
    string? Code,
    AttributeValueType? ValueType,
    string? Unit
) : IRequest<Result>;
