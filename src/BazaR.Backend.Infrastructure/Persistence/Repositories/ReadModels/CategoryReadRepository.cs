using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class CategoryReadRepository : ICategoryReadRepository
{
    private readonly AppDbContext _db;

    public CategoryReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<CategoryDetailsDto?> GetByIdAsync(CategoryId id, CancellationToken ct)
        => _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDetailsDto(
                c.Id.Value,
                c.Name,
                c.ParentCategoryId.HasValue
                    ? c.ParentCategoryId.Value.Value
                    : (Guid?)null,
                c.SortOrder,
                c.Image != null ? c.Image.Url : null,
                c.Attributes
                    .OrderBy(a => a.SectionOrder ?? int.MaxValue)
                    .ThenBy(a => a.SectionName)
                    .ThenBy(a => a.SortOrder)
                    .Select(a => new CategoryAttributeTemplateItemDto(
                        a.AttributeId.Value,
                        a.IsRequired,
                        a.IsFilterable,
                        a.SortOrder,
                        a.SectionName,
                        a.SectionOrder
                    ))
                    .ToList()
            ))
            .SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<CategoryListItemDto>> ListAsync(CancellationToken ct)
        => await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.ParentCategoryId == null ? 0 : 1)
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryListItemDto(
                c.Id.Value,
                c.Name,
                c.ParentCategoryId.HasValue
                    ? c.ParentCategoryId.Value.Value
                    : (Guid?)null,
                c.SortOrder,
                c.Attributes.Count,
                c.Image != null ? c.Image.Url : null
            ))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CategoryListItemDto>> SearchAsync(string term, int limit, CancellationToken ct)
    {
        term ??= string.Empty;
        var trimmed = term.Trim();

        if (limit <= 0) limit = 10;
        if (limit > 50) limit = 50;

        IQueryable<Category> query = _db.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            var pattern = $"%{trimmed}%";
            query = query.Where(c => EF.Functions.ILike(c.Name, pattern));
        }

        return await query
            .OrderBy(c => c.Name)
            .Take(limit)
            .Select(c => new CategoryListItemDto(
                c.Id.Value,
                c.Name,
                c.ParentCategoryId.HasValue
                    ? c.ParentCategoryId.Value.Value
                    : (Guid?)null,
                c.SortOrder,
                c.Attributes.Count,
                c.Image != null ? c.Image.Url : null
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CategoryTreeNodeDto>> GetTreeAsync(CancellationToken ct)
        => await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.ParentCategoryId == null ? 0 : 1)
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryTreeNodeDto(
                c.Id.Value,
                c.Name,
                c.ParentCategoryId.HasValue
                    ? c.ParentCategoryId.Value.Value
                    : (Guid?)null,
                c.SortOrder
            ))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CategoryAttributeTemplateItemDto>> GetAttributesTemplateAsync(CategoryId id, CancellationToken ct)
        => await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .SelectMany(c => c.Attributes)
            .OrderBy(a => a.SectionOrder ?? int.MaxValue)
            .ThenBy(a => a.SectionName)
            .ThenBy(a => a.SortOrder)
            .Select(a => new CategoryAttributeTemplateItemDto(
                a.AttributeId.Value,
                a.IsRequired,
                a.IsFilterable,
                a.SortOrder,
                a.SectionName,
                a.SectionOrder
            ))
            .ToListAsync(ct);
}