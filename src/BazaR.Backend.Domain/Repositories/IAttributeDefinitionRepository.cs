using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IAttributeDefinitionRepository
{
    Task<AttributeDefinition?> GetByIdAsync(AttributeId id, CancellationToken ct);
    Task<AttributeDefinition?> GetByCodeAsync(string code, CancellationToken ct);

    Task<bool> ExistsAsync(AttributeId id, CancellationToken ct);
    Task<bool> CodeExistsAsync(string code, AttributeId? excludeId, CancellationToken ct);

    Task AddAsync(AttributeDefinition attribute, CancellationToken ct);
    Task RemoveAsync(AttributeDefinition attribute, CancellationToken ct);


    Task<IReadOnlyList<AttributeDefinition>> GetByIdsAsync(
        IReadOnlyCollection<AttributeId> ids,
        CancellationToken ct);
}
