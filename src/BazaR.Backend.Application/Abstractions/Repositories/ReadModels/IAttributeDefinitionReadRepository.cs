using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface IAttributeDefinitionReadRepository
{
    Task<AttributeDefinitionDetailsDto?> GetByIdAsync(AttributeId id, CancellationToken ct);

    
    // Пока без фильтров просто все элементы для таблицы
    Task<IReadOnlyList<AttributeDefinitionListItemDto>> ListAsync(CancellationToken ct);

   
    // Для autocomplete (по name/code)
    Task<IReadOnlyList<AttributeDefinitionListItemDto>> SearchAsync(
        string term,
        int limit,
        CancellationToken ct);

    Task<IReadOnlyList<AttributeOptionDto>> GetOptionsAsync(AttributeId id, CancellationToken ct);
}
