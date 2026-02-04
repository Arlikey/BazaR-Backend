using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence;

public sealed class AttributeUsageChecker : IAttributeUsageChecker
{
    private readonly AppDbContext _db;

    public AttributeUsageChecker(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> IsUsedInCategoriesAsync(AttributeId attributeId, CancellationToken ct)
        => _db.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Attributes.Any(a => a.AttributeId == attributeId), ct);

    public Task<bool> IsUsedInProductsAsync(AttributeId attributeId, CancellationToken ct)
        => _db.Products
            .AsNoTracking()
            .AnyAsync(p => p.AttributeValues.Any(v => v.AttributeId == attributeId), ct);
}
