using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class AttributeDefinitionReadRepository : IAttributeDefinitionReadRepository
{
    private readonly AppDbContext _db;

    public AttributeDefinitionReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<AttributeDefinitionDetailsDto?> GetByIdAsync(AttributeId id, CancellationToken ct)
        => _db.AttributeDefinitions
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new AttributeDefinitionDetailsDto(
                a.Id.Value,
                a.Name,
                a.Code,
                a.ValueType,
                a.Unit,
                a.IsSystem,
                a.Options
                    .OrderBy(o => o.Value)
                    .Select(o => new AttributeOptionDto(o.Id, o.Value))
                    .ToList()
            ))
            .SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<AttributeDefinitionListItemDto>> ListAsync(CancellationToken ct)
        => await _db.AttributeDefinitions
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new AttributeDefinitionListItemDto(
                a.Id.Value,
                a.Name,
                a.Code,
                a.ValueType,
                a.Unit,
                a.IsSystem,
                a.Options.Count
            ))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<AttributeDefinitionListItemDto>> SearchAsync(
        string term,
        int limit,
        CancellationToken ct)
    {
        term ??= string.Empty;
        var trimmed = term.Trim();

        if (limit <= 0) limit = 10;
        if (limit > 50) limit = 50;

        IQueryable<AttributeDefinition> query = _db.AttributeDefinitions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(trimmed))
        {
            var pattern = $"%{trimmed}%";

          
            query = query.Where(a =>
                EF.Functions.ILike(a.Name, pattern) ||
                EF.Functions.ILike(a.Code, pattern));
        }

        return await query
            .OrderBy(a => a.Name)
            .Take(limit)
            .Select(a => new AttributeDefinitionListItemDto(
                a.Id.Value,
                a.Name,
                a.Code,
                a.ValueType,
                a.Unit,
                a.IsSystem,
                a.Options.Count
            ))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AttributeOptionDto>> GetOptionsAsync(AttributeId id, CancellationToken ct)
        => await _db.AttributeDefinitions
            .AsNoTracking()
            .Where(a => a.Id == id)
            .SelectMany(a => a.Options)
            .OrderBy(o => o.Value)
            .Select(o => new AttributeOptionDto(o.Id, o.Value))
            .ToListAsync(ct);
}
