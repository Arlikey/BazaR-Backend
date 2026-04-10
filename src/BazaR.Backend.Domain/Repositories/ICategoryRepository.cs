using BazaR.Backend.Domain.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct);
    Task<bool> ExistsAsync(CategoryId id, CancellationToken ct);
    Task<bool> SlugExistsAsync(
    CategorySlug slug,
    CategoryId? excludeCategoryId,
    CancellationToken ct = default);
    Task<Category?> GetBySlugAsync(CategorySlug slug, CancellationToken ct);
    Task<IReadOnlyList<Category>> GetChildrenAsync(CategoryId? parentId, CancellationToken ct);
    Task<bool> NameExistsAsync(string name, CategoryId? parentId, CategoryId? excludeCategoryId, CancellationToken ct);

    Task<bool> IsDescendantAsync(CategoryId ancestorId, CategoryId possibleDescendantId, CancellationToken ct);
    Task<bool> HasChildrenAsync(CategoryId parentId, CancellationToken ct);

    Task<IReadOnlyList<Category>> GetPathAsync(CategoryId categoryId, CancellationToken ct);
    Task<IReadOnlyList<Category>> GetTreeAsync(CancellationToken ct);

    Task AddAsync(Category category, CancellationToken ct);
    Task RemoveAsync(Category category, CancellationToken ct);

   ///новые
    Task<IReadOnlyList<Category>> GetParentCategoriesAsync(CancellationToken ct); 
    Task<IReadOnlyList<Category>> GetSubcategoriesByParentIdAsync(CategoryId parentId, CancellationToken ct);
    Task<Category?> GetByIdWithAttributesAsync(CategoryId id, CancellationToken ct);

}
