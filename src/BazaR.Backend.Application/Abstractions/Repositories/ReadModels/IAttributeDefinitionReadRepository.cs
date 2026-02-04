using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IAttributeDefinitionReadRepository
{
    Task<AttributeDefinitionDetailsDto?> GetByIdAsync(AttributeId id, CancellationToken ct);

    /// <summary>
    /// Пока без фильтров просто все элементы для таблицы
    /// </summary>
    Task<IReadOnlyList<AttributeDefinitionListItemDto>> ListAsync(CancellationToken ct);

    /// <summary>
    /// Для autocomplete (по name/code)
    /// </summary>
    Task<IReadOnlyList<AttributeDefinitionListItemDto>> SearchAsync(
        string term,
        int limit,
        CancellationToken ct);

    Task<IReadOnlyList<AttributeOptionDto>> GetOptionsAsync(AttributeId id, CancellationToken ct);
}
