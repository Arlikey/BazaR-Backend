using BazaR.Backend.Domain.Categories;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface ICategoryReadRepository
{
    Task<CategoryDetailsDto?> GetByIdAsync(CategoryId id, CancellationToken ct);

    /// <summary>
    /// Пока без фильтров: список для таблицы
    /// </summary>
    Task<IReadOnlyList<CategoryListItemDto>> ListAsync(CancellationToken ct);

    Task<IReadOnlyList<CategoryListItemDto>> SearchAsync(string term, int limit, CancellationToken ct);

    /// <summary>
    /// Дерево/структура: все категории (потом на фронте строишь дерево).
    /// </summary>
    Task<IReadOnlyList<CategoryTreeNodeDto>> GetTreeAsync(CancellationToken ct);

    /// <summary>
    /// Только шаблон атрибутов категории (если UI грузит отдельно)
    /// </summary>
    Task<IReadOnlyList<CategoryAttributeTemplateItemDto>> GetAttributesTemplateAsync(CategoryId id, CancellationToken ct);
}
