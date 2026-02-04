using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Api.Contracts.Admin.Catalog.Attributes;

public sealed record CreateAttributeDefinitionRequest(
    string Name,
    string Code,
    AttributeValueType ValueType,
    string? Unit,
    bool IsSystem = false
);
