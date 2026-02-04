using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class AttributeDefinitionRepository : IAttributeDefinitionRepository
{
    private readonly AppDbContext _db;

    public AttributeDefinitionRepository(AppDbContext db)
    {
        _db = db;
    }

    
    public Task<AttributeDefinition?> GetByIdAsync(AttributeId id, CancellationToken ct)
        => _db.AttributeDefinitions
            .Include(a => a.Options)
            .SingleOrDefaultAsync(a => a.Id == id, ct);

    // Обычно полезно для интеграций/поиска
    public Task<AttributeDefinition?> GetByCodeAsync(string code, CancellationToken ct)
    {
        var normalized = NormalizeCode(code);

        return _db.AttributeDefinitions
            .Include(a => a.Options)
            .SingleOrDefaultAsync(a => a.Code == normalized, ct);
    }

    public Task<bool> ExistsAsync(AttributeId id, CancellationToken ct)
        => _db.AttributeDefinitions.AnyAsync(a => a.Id == id, ct);

    public Task<bool> CodeExistsAsync(string code, AttributeId? excludeId, CancellationToken ct)
    {
        var normalized = NormalizeCode(code);

        var query = _db.AttributeDefinitions.Where(a => a.Code == normalized);

        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);

        return query.AnyAsync(ct);
    }

    public async Task AddAsync(AttributeDefinition attribute, CancellationToken ct)
        => await _db.AttributeDefinitions.AddAsync(attribute, ct);

    public Task RemoveAsync(AttributeDefinition attribute, CancellationToken ct)
    {
        _db.AttributeDefinitions.Remove(attribute);
        return Task.CompletedTask;
    }

    private static string NormalizeCode(string code)
        => code.Trim().ToLowerInvariant();

    public async Task<IReadOnlyList<AttributeDefinition>> GetByIdsAsync(
       IReadOnlyCollection<AttributeId> ids,
       CancellationToken ct)
    {
        if (ids is null || ids.Count == 0)
            return Array.Empty<AttributeDefinition>();

        
        return await _db.AttributeDefinitions
            .AsNoTracking()
            .Include(a => a.Options)
            .AsSplitQuery()
            .Where(a => ids.Contains(a.Id))
            .ToListAsync(ct);
    }
}
