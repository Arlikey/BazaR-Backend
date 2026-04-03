using BazaR.Backend.Application.Catalog.Browsing.Abstractions;
using BazaR.Backend.Application.Catalog.Browsing.DTOs;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Repositories;

public sealed class CatalogBrowseReadRepository : ICatalogBrowseReadRepository
{
    private readonly AppDbContext _db;

    public CatalogBrowseReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CategoryCatalogSidebarDto> GetCategorySidebarAsync(
        Guid categoryId,
        CancellationToken ct = default)
    {
        var categoryAttributes = await LoadFilterableCategoryAttributeMetasAsync(categoryId, ct);

        if (categoryAttributes.Count == 0)
        {
            return new CategoryCatalogSidebarDto
            {
                CategoryId = categoryId,
                Facets = []
            };
        }

        var attributeIds = categoryAttributes
            .Select(x => x.AttributeId)
            .Distinct()
            .ToList();

        var definitions = await LoadAttributeDefinitionMetasAsync(attributeIds, ct);
        var definitionMap = definitions.ToDictionary(x => x.AttributeId);

        var products = await LoadCategoryPublishedProductsAsync(categoryId, ct);

        var facets = new List<CatalogFacetDto>();

        foreach (var categoryAttribute in categoryAttributes
                     .OrderBy(x => x.SectionOrder ?? int.MaxValue)
                     .ThenBy(x => x.SectionName)
                     .ThenBy(x => x.SortOrder))
        {
            if (!definitionMap.TryGetValue(categoryAttribute.AttributeId, out var definition))
                continue;

            var facet = BuildFacet(products, categoryAttribute, definition);

            if (facet is not null)
                facets.Add(facet);
        }

        return new CategoryCatalogSidebarDto
        {
            CategoryId = categoryId,
            Facets = facets
        };
    }

    private async Task<List<CategoryAttributeMeta>> LoadFilterableCategoryAttributeMetasAsync(
        Guid categoryId,
        CancellationToken ct)
    {
        var categoryVo = new CategoryId(categoryId);

        return await _db.Categories
            .AsNoTracking()
            .Where(x => x.Id == categoryVo)
            .SelectMany(x => x.Attributes
                .Where(a => a.IsFilterable)
                .Select(a => new CategoryAttributeMeta
                {
                    AttributeId = a.AttributeId.Value,
                    FilterPresentationType = a.FilterPresentationType,
                    SectionName = a.SectionName,
                    SectionOrder = a.SectionOrder,
                    SortOrder = a.SortOrder
                }))
            .ToListAsync(ct);
    }

    private async Task<List<AttributeDefinitionMeta>> LoadAttributeDefinitionMetasAsync(
        IReadOnlyCollection<Guid> attributeIds,
        CancellationToken ct)
    {
        if (attributeIds.Count == 0)
            return [];

        var attributeIdSet = attributeIds.ToHashSet();

        var allDefinitions = await _db.AttributeDefinitions
            .AsNoTracking()
            .Select(x => new AttributeDefinitionMeta
            {
                AttributeId = x.Id.Value,
                Code = x.Code,
                Name = x.Name,
                ValueType = x.ValueType,
                Unit = x.Unit,
                Options = x.Options
                    .OrderBy(o => o.SortOrder)
                    .Select(o => new AttributeOptionMeta
                    {
                        Id = o.Id,
                        Value = o.Value,
                        SortOrder = o.SortOrder
                    })
                    .ToList()
            })
            .ToListAsync(ct);

        return allDefinitions
            .Where(x => attributeIdSet.Contains(x.AttributeId))
            .ToList();
    }

    private async Task<List<ProductSidebarData>> LoadCategoryPublishedProductsAsync(
        Guid categoryId,
        CancellationToken ct)
    {
        var categoryVo = new CategoryId(categoryId);

        var productIds = await _db.Products
            .AsNoTracking()
            .Where(p => p.Status == ProductStatus.Published)
            .Where(p => p.CategoryId == categoryVo)
            .Select(p => p.Id.Value)
            .ToListAsync(ct);

        if (productIds.Count == 0)
            return [];

        var productIdSet = productIds.ToHashSet();

        var attributeValueRows = await _db.Products
            .AsNoTracking()
            .Where(p => p.Status == ProductStatus.Published)
            .Where(p => p.CategoryId == categoryVo)
            .SelectMany(p => p.AttributeValues.Select(v => new ProductAttributeValueRow
            {
                ProductId = p.Id.Value,
                ProductAttributeValueId = v.Id,
                AttributeId = v.AttributeId.Value,
                TextValue = v.TextValue,
                NumberValue = v.NumberValue,
                BoolValue = v.BoolValue,
                OptionId = v.OptionId
            }))
            .ToListAsync(ct);

        var optionRows = await _db.Products
            .AsNoTracking()
            .Where(p => p.Status == ProductStatus.Published)
            .Where(p => p.CategoryId == categoryVo)
            .SelectMany(p => p.AttributeValues)
            .SelectMany(v => v.OptionIds.Select(o => new ProductAttributeValueOptionRow
            {
                ProductAttributeValueId = v.Id,
                OptionId = o.OptionId
            }))
            .ToListAsync(ct);

        var optionMap = optionRows
            .GroupBy(x => x.ProductAttributeValueId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.OptionId).ToList());

        var groupedProducts = attributeValueRows
            .Where(x => productIdSet.Contains(x.ProductId))
            .GroupBy(x => x.ProductId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var products = productIds
            .Select(productId =>
            {
                groupedProducts.TryGetValue(productId, out var values);

                return new ProductSidebarData
                {
                    ProductId = productId,
                    AttributeValues = (values ?? new List<ProductAttributeValueRow>())
                        .Select(v => new ProductAttributeValueData
                        {
                            AttributeId = v.AttributeId,
                            TextValue = v.TextValue,
                            NumberValue = v.NumberValue,
                            BoolValue = v.BoolValue,
                            OptionId = v.OptionId,
                            OptionIds = optionMap.TryGetValue(v.ProductAttributeValueId, out var ids)
                                ? ids
                                : []
                        })
                        .ToList()
                };
            })
            .ToList();

        return products;
    }

    private CatalogFacetDto? BuildFacet(
        IReadOnlyCollection<ProductSidebarData> products,
        CategoryAttributeMeta categoryMeta,
        AttributeDefinitionMeta definition)
    {
        return definition.ValueType switch
        {
            AttributeValueType.Boolean => BuildBooleanFacet(products, categoryMeta, definition),
            AttributeValueType.Select => BuildSelectFacet(products, categoryMeta, definition),
            AttributeValueType.MultiSelect => BuildMultiSelectFacet(products, categoryMeta, definition),
            AttributeValueType.Number => BuildNumberFacet(products, categoryMeta, definition),
            _ => null
        };
    }

    private CatalogFacetDto BuildBooleanFacet(
        IReadOnlyCollection<ProductSidebarData> products,
        CategoryAttributeMeta categoryMeta,
        AttributeDefinitionMeta definition)
    {
        var counts = products
            .SelectMany(p => p.AttributeValues
                .Where(v => v.AttributeId == definition.AttributeId && v.BoolValue.HasValue)
                .Select(v => new
                {
                    p.ProductId,
                    Value = v.BoolValue!.Value
                }))
            .GroupBy(x => x.Value)
            .Select(g => new
            {
                Value = g.Key,
                Count = g.Select(x => x.ProductId).Distinct().Count()
            })
            .ToDictionary(x => x.Value, x => x.Count);

        return new CatalogFacetDto
        {
            Kind = "attribute",
            AttributeId = definition.AttributeId,
            Code = definition.Code,
            Name = definition.Name,
            Type = "boolean",
            Unit = definition.Unit,
            SectionName = categoryMeta.SectionName,
            SectionOrder = categoryMeta.SectionOrder,
            SortOrder = categoryMeta.SortOrder,
            Options = new List<CatalogFacetOptionDto>
            {
                new()
                {
                    Value = "true",
                    Label = "Да",
                    Count = counts.TryGetValue(true, out var trueCount) ? trueCount : 0,
                    Selected = false
                },
                new()
                {
                    Value = "false",
                    Label = "Нет",
                    Count = counts.TryGetValue(false, out var falseCount) ? falseCount : 0,
                    Selected = false
                }
            }
        };
    }

    private CatalogFacetDto BuildSelectFacet(
        IReadOnlyCollection<ProductSidebarData> products,
        CategoryAttributeMeta categoryMeta,
        AttributeDefinitionMeta definition)
    {
        var counts = products
            .SelectMany(p => p.AttributeValues
                .Where(v => v.AttributeId == definition.AttributeId && v.OptionId.HasValue)
                .Select(v => new
                {
                    p.ProductId,
                    OptionId = v.OptionId!.Value
                }))
            .GroupBy(x => x.OptionId)
            .Select(g => new
            {
                OptionId = g.Key,
                Count = g.Select(x => x.ProductId).Distinct().Count()
            })
            .ToDictionary(x => x.OptionId, x => x.Count);

        var options = definition.Options
            .Select(opt => new CatalogFacetOptionDto
            {
                Value = opt.Id.ToString(),
                Label = opt.Value,
                Count = counts.TryGetValue(opt.Id, out var count) ? count : 0,
                Selected = false
            })
            .ToList();

        return new CatalogFacetDto
        {
            Kind = "attribute",
            AttributeId = definition.AttributeId,
            Code = definition.Code,
            Name = definition.Name,
            Type = "select",
            Unit = definition.Unit,
            SectionName = categoryMeta.SectionName,
            SectionOrder = categoryMeta.SectionOrder,
            SortOrder = categoryMeta.SortOrder,
            Options = options
        };
    }

    private CatalogFacetDto BuildMultiSelectFacet(
     IReadOnlyCollection<ProductSidebarData> products,
     CategoryAttributeMeta categoryMeta,
     AttributeDefinitionMeta definition)
    {
        Console.WriteLine($"===== MULTI SELECT FACET: {definition.Code} / {definition.AttributeId} =====");

        foreach (var product in products)
        {
            var matchingValues = product.AttributeValues
                .Where(value => value.AttributeId == definition.AttributeId)
                .ToList();

            if (matchingValues.Count == 0)
                continue;

            Console.WriteLine($"Product: {product.ProductId}");

            foreach (var value in matchingValues)
            {
                Console.WriteLine($"  AttributeId: {value.AttributeId}");
                Console.WriteLine($"  OptionIds: {(value.OptionIds.Count == 0 ? "<empty>" : string.Join(", ", value.OptionIds))}");
            }
        }

        Console.WriteLine("Definition options:");
        foreach (var option in definition.Options.OrderBy(x => x.SortOrder))
        {
            Console.WriteLine($"  {option.Value} / {option.Id}");
        }

        var countsByOptionId = products
            .SelectMany(product => product.AttributeValues
                .Where(value => value.AttributeId == definition.AttributeId)
                .SelectMany(value => value.OptionIds.Distinct(), (value, optionId) => new
                {
                    product.ProductId,
                    OptionId = optionId
                }))
            .GroupBy(x => x.OptionId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ProductId).Distinct().Count());

        Console.WriteLine("Counts by option id:");
        foreach (var pair in countsByOptionId)
        {
            Console.WriteLine($"  {pair.Key} => {pair.Value}");
        }

        var options = definition.Options
            .OrderBy(x => x.SortOrder)
            .Select(option => new CatalogFacetOptionDto
            {
                Value = option.Id.ToString(),
                Label = option.Value,
                Count = countsByOptionId.TryGetValue(option.Id, out var count) ? count : 0,
                Selected = false
            })
            .ToList();

        return new CatalogFacetDto
        {
            Kind = "attribute",
            AttributeId = definition.AttributeId,
            Code = definition.Code,
            Name = definition.Name,
            Type = "multi_select",
            Unit = definition.Unit,
            SectionName = categoryMeta.SectionName,
            SectionOrder = categoryMeta.SectionOrder,
            SortOrder = categoryMeta.SortOrder,
            Options = options
        };
    }

    private CatalogFacetDto BuildNumberFacet(
        IReadOnlyCollection<ProductSidebarData> products,
        CategoryAttributeMeta categoryMeta,
        AttributeDefinitionMeta definition)
    {
        var values = products
            .SelectMany(p => p.AttributeValues
                .Where(v => v.AttributeId == definition.AttributeId && v.NumberValue.HasValue)
                .Select(v => v.NumberValue!.Value))
            .ToList();

        decimal? min = values.Count > 0 ? values.Min() : null;
        decimal? max = values.Count > 0 ? values.Max() : null;

        return new CatalogFacetDto
        {
            Kind = "attribute",
            AttributeId = definition.AttributeId,
            Code = definition.Code,
            Name = definition.Name,
            Type = "range",
            Unit = definition.Unit,
            SectionName = categoryMeta.SectionName,
            SectionOrder = categoryMeta.SectionOrder,
            SortOrder = categoryMeta.SortOrder,
            Min = min,
            Max = max,
            Options = []
        };
    }

    private sealed class CategoryAttributeMeta
    {
        public Guid AttributeId { get; init; }
        public FilterPresentationType? FilterPresentationType { get; init; }
        public string? SectionName { get; init; }
        public int? SectionOrder { get; init; }
        public int SortOrder { get; init; }
    }

    private sealed class AttributeDefinitionMeta
    {
        public Guid AttributeId { get; init; }
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public AttributeValueType ValueType { get; init; }
        public string? Unit { get; init; }
        public List<AttributeOptionMeta> Options { get; init; } = [];
    }

    private sealed class AttributeOptionMeta
    {
        public Guid Id { get; init; }
        public string Value { get; init; } = default!;
        public int SortOrder { get; init; }
    }

    private sealed class ProductAttributeValueRow
    {
        public Guid ProductId { get; init; }
        public Guid ProductAttributeValueId { get; init; }
        public Guid AttributeId { get; init; }
        public string? TextValue { get; init; }
        public decimal? NumberValue { get; init; }
        public bool? BoolValue { get; init; }
        public Guid? OptionId { get; init; }
    }

    private sealed class ProductAttributeValueOptionRow
    {
        public Guid ProductAttributeValueId { get; init; }
        public Guid OptionId { get; init; }
    }

    private sealed class ProductSidebarData
    {
        public Guid ProductId { get; init; }
        public List<ProductAttributeValueData> AttributeValues { get; init; } = [];
    }

    private sealed class ProductAttributeValueData
    {
        public Guid AttributeId { get; init; }
        public string? TextValue { get; init; }
        public decimal? NumberValue { get; init; }
        public bool? BoolValue { get; init; }
        public Guid? OptionId { get; init; }
        public List<Guid> OptionIds { get; init; } = [];
    }
}