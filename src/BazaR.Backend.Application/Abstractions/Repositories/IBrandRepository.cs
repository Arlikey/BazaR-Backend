using BazaR.Backend.Domain.Brands;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(BrandId id, CancellationToken ct = default);
    Task<Brand?> GetBySlugAsync(string slug, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);

    Task AddAsync(Brand brand, CancellationToken ct = default);
}