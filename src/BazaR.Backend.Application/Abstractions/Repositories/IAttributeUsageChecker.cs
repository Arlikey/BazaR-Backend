using BazaR.Backend.Domain.Catalog.Attributes;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IAttributeUsageChecker
{
    Task<bool> IsUsedInCategoriesAsync(AttributeId attributeId, CancellationToken ct);
    Task<bool> IsUsedInProductsAsync(AttributeId attributeId, CancellationToken ct);
}
