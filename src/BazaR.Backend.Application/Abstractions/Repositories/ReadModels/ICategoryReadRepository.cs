using BazaR.Backend.Domain.Categories;

namespace BazaR.Backend.Application.Abstractions.ReadModels;

public interface ICategoryReadRepository
{
    Task<CategoryDetailsDto?> GetByIdAsync(CategoryId id, CancellationToken ct);
    Task<IReadOnlyList<CategoryListItemDto>> ListAsync(CancellationToken ct);

    Task<IReadOnlyList<CategoryListItemDto>> SearchAsync(string term, int limit, CancellationToken ct);

    
    // Дерево/структура: все категории (потом на фронте строить дерево).
    Task<IReadOnlyList<CategoryTreeNodeDto>> GetTreeAsync(CancellationToken ct);

    Task<IReadOnlyList<CategoryAttributeTemplateItemDto>> GetAttributesTemplateAsync(CategoryId id, CancellationToken ct);
}
