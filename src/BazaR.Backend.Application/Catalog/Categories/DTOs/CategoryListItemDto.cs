using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

// Для таблицы / списка
public sealed record CategoryListItemDto(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    int SortOrder,
    int AttributesCount,
    string? ImageUrl);

// Детали категории + шаблон атрибутов
public sealed record CategoryDetailsDto(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    int SortOrder,
    string? ImageUrl,
    IReadOnlyList<CategoryAttributeTemplateItemDto> Attributes);

public sealed record CategoryAttributeTemplateItemDto(
    Guid AttributeId,
    bool IsRequired,
    bool IsFilterable,
    int SortOrder,
    string? SectionName,
    int? SectionOrder);

// Для дерева (простая форма)
public sealed record CategoryTreeNodeDto(
    Guid Id,
    string Name,
    Guid? ParentCategoryId,
    int SortOrder);
