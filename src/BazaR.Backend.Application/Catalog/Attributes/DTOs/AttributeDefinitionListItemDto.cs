using BazaR.Backend.Domain.Catalog.Attributes;

public sealed record AttributeDefinitionListItemDto(
    Guid Id,
    string Name,
    string Code,
    AttributeValueType ValueType,
    string? Unit,
    bool IsSystem,
    int OptionsCount);
