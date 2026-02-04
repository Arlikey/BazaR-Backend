using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


public sealed class ProductAttributesReadService : IProductAttributesReadService
{
    private readonly AppDbContext _db;

    public ProductAttributesReadService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProductAttributesViewDto?> GetAttributesViewAsync(
        ProductId productId,
        CancellationToken ct)
    {
        // 1) Загружаем продукт с attribute values
        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.AttributeValues)
            .SingleOrDefaultAsync(p => p.Id == productId, ct);

        if (product is null)
            return null;

        var categoryId = product.CategoryId;

        // 2) Загружаем template категории
        var template = await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == categoryId)
            .SelectMany(c => c.Attributes)
            .Select(a => new
            {
                a.AttributeId,
                a.IsRequired,
                a.IsFilterable,
                a.SortOrder,
                a.SectionName,
                a.SectionOrder
            })
            .ToListAsync(ct);

        var attrIds = template.Select(x => x.AttributeId).Distinct().ToList();

        // 3) Загружаем AttributeDefinitions + Options
        var defs = await _db.AttributeDefinitions
            .AsNoTracking()
            .Include(d => d.Options)
            .Where(d => attrIds.Contains(d.Id))
            .ToListAsync(ct);

        var defMap = defs.ToDictionary(d => d.Id, d => d);

        // 4) Map ProductAttributeValues из агрегата
        var valueMap = product.AttributeValues
            .ToDictionary(v => v.AttributeId, v => v);

        // 5) Compose DTO
        var items = template
            .OrderBy(x => x.SectionOrder ?? int.MaxValue)
            .ThenBy(x => x.SectionName)
            .ThenBy(x => x.SortOrder)
            .Select(t =>
            {
                defMap.TryGetValue(t.AttributeId, out var def);
                valueMap.TryGetValue(t.AttributeId, out var val);

                return new ProductAttributeViewItemDto(
                    AttributeId: t.AttributeId.Value,
                    Name: def?.Name ?? "(unknown)",
                    Code: def?.Code ?? "(unknown)",
                    ValueType: def?.ValueType.ToString() ?? "Unknown",
                    Unit: def?.Unit,

                    IsRequired: t.IsRequired,
                    IsFilterable: t.IsFilterable,
                    SortOrder: t.SortOrder,
                    SectionName: t.SectionName,
                    SectionOrder: t.SectionOrder,

                    TextValue: val?.TextValue,
                    NumberValue: val?.NumberValue,
                    BoolValue: val?.BoolValue,
                    OptionId: val?.OptionId,
                    OptionIds: val?.OptionIds?.ToList() ?? (IReadOnlyList<Guid>)Array.Empty<Guid>(),

                    Options: def?.Options
                        .Select(o => new AttributeOptionDto(o.Id, o.Value))
                        .ToList()
                        ?? new List<AttributeOptionDto>()
                );
            })
            .ToList();

        return new ProductAttributesViewDto(
            product.Id.Value,
            categoryId.Value,
            items);
    }
}
