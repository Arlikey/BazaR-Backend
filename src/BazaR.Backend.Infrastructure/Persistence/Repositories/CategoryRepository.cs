using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    
    // Лёгкая загрузка категории (tracked), без атрибутов.
    // Подходит для Rename/Move и других операций, не трогающих Attributes.
    public Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct)
        => _db.Categories
            .SingleOrDefaultAsync(c => c.Id == id, ct);

    // Полная загрузка агрегата (tracked) с Category.Attributes.
    // Используй для AddAttribute/RemoveAttribute/UpdateAttributeRules/SetSection.
    public Task<Category?> GetByIdWithAttributesAsync(CategoryId id, CancellationToken ct)
        => _db.Categories
            .Include(c => c.Attributes)
            .SingleOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExistsAsync(CategoryId id, CancellationToken ct)
        => _db.Categories.AnyAsync(c => c.Id == id, ct);

    // =====================================================
    // Read-side: hierarchy
    // =====================================================

    public async Task<IReadOnlyList<Category>> GetParentCategoriesAsync(CancellationToken ct)
        => await _db.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Category>> GetSubcategoriesByParentIdAsync(CategoryId parentId, CancellationToken ct)
        => await _db.Categories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == parentId)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Category>> GetChildrenAsync(CategoryId? parentId, CancellationToken ct)
    {
        var query = _db.Categories.AsNoTracking();

        query = parentId is null
            ? query.Where(c => c.ParentCategoryId == null)
            : query.Where(c => c.ParentCategoryId == parentId);

        return await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
    }

    public Task<bool> HasChildrenAsync(CategoryId parentId, CancellationToken ct)
        => _db.Categories.AnyAsync(c => c.ParentCategoryId == parentId, ct);

    public async Task<IReadOnlyList<Category>> GetPathAsync(CategoryId categoryId, CancellationToken ct)
    {
        var path = new List<Category>();
        var currentId = categoryId;

        while (true)
        {
            var category = await _db.Categories
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == currentId, ct);

            if (category is null)
                break;

            path.Insert(0, category);

            if (category.ParentCategoryId is null)
                break;

            currentId = category.ParentCategoryId.Value;
        }

        return path;
    }

    public async Task<IReadOnlyList<Category>> GetTreeAsync(CancellationToken ct)
        => await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.ParentCategoryId == null ? 0 : 1)
            .ThenBy(c => c.ParentCategoryId)
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);

    // =====================================================
    // Validation helpers
    // =====================================================

    public Task<bool> NameExistsAsync(
        string name,
        CategoryId? parentId,
        CategoryId? excludeCategoryId,
        CancellationToken ct)
    {
        var trimmed = name.Trim();

        var query = _db.Categories
            .Where(c => c.Name == trimmed && c.ParentCategoryId == parentId);

        if (excludeCategoryId.HasValue)
            query = query.Where(c => c.Id != excludeCategoryId.Value);

        return query.AnyAsync(ct);
    }

    public async Task<bool> IsDescendantAsync(CategoryId ancestorId, CategoryId possibleDescendantId, CancellationToken ct)
    {
        if (ancestorId == possibleDescendantId)
            return true;

        var current = possibleDescendantId;

        while (true)
        {
            var parent = await _db.Categories
                .AsNoTracking()
                .Where(c => c.Id == current)
                .Select(c => c.ParentCategoryId)
                .SingleOrDefaultAsync(ct);

            if (parent is null)
                return false;

            if (parent.Value == ancestorId)
                return true;

            current = parent.Value;
        }
    }

    // =====================================================
    // Persistence
    // =====================================================

    public async Task AddAsync(Category category, CancellationToken ct)
        => await _db.Categories.AddAsync(category, ct);

    public Task RemoveAsync(Category category, CancellationToken ct)
    {
        _db.Categories.Remove(category);
        return Task.CompletedTask;
    }
}
