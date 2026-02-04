using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Attributes.Commands.CreateAttributeDefinition;

public sealed record CreateAttributeDefinitionCommand(
    string Name,
    string Code,
    AttributeValueType ValueType,
    string? Unit,
    bool IsSystem = false
) : IRequest<Result<Guid>>;
