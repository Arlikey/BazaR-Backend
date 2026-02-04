using BazaR.Backend.Domain.Catalog.Attributes;

public sealed record AttributeDefinitionDetailsDto(
    Guid Id,
    string Name,
    string Code,
    AttributeValueType ValueType,
    string? Unit,
    bool IsSystem,
    IReadOnlyList<AttributeOptionDto> Options);

public sealed record AttributeOptionDto(Guid Id, string Value);
