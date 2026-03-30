using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class SellerRepository : ISellerRepository
{
    private readonly AppDbContext _db;

    public SellerRepository(AppDbContext db) => _db = db;

    public void Add(Seller seller) => _db.Sellers.Add(seller);
    public void Update(Seller seller) => _db.Sellers.Update(seller);
    public void Remove(Seller seller) => _db.Sellers.Remove(seller);

    public async Task<Seller?> GetByIdAsync(SellerId id, CancellationToken ct = default)
        => await _db.Sellers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<bool> ExistsAsync(SellerId id, CancellationToken ct = default)
        => await _db.Sellers.AnyAsync(x => x.Id == id, ct);

    public async Task<Seller?> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken ct = default)

    {
        if (ownerUserId == Guid.Empty) return null;

        return await _db.Sellers.FirstOrDefaultAsync(x => x.OwnerUserId == ownerUserId, ct);
    }

    public async Task<Seller?> GetByOwnerUserIdAsync(UserId userId, CancellationToken ct = default)
    {
        return await _db.Sellers.FirstOrDefaultAsync(x => x.OwnerUserId == userId.Value, ct);
    }


    public async Task<Seller?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var normalized = NormalizeSlug(slug);
        if (normalized is null) return null;

        return await _db.Sellers.FirstOrDefaultAsync(x => x.Slug.Value == normalized, ct);
    }

    public async Task<bool> SlugExistsAsync(SellerSlug slug, SellerId? excludeSellerId = null, CancellationToken ct = default)
    {
        if (slug is null) throw new ArgumentNullException(nameof(slug));

        var q = _db.Sellers.Where(x => x.Slug.Value == slug.Value);

        if (excludeSellerId is not null)
        {
            var excluded = excludeSellerId.Value;
            q = q.Where(x => x.Id != excluded);
        }

        return await q.AnyAsync(ct);
    }

    public async Task<bool> TaxNumberExistsAsync(string taxNumber, SellerId? excludeSellerId = null, CancellationToken ct = default)
    {
        var normalized = NormalizeTaxNumber(taxNumber);
        if (normalized is null) return false;

        var q = _db.Sellers.Where(x => x.TaxNumber != null && x.TaxNumber == normalized);

        if (excludeSellerId is not null)
        {
            var excluded = excludeSellerId.Value;
            q = q.Where(x => x.Id != excluded);
        }

        return await q.AnyAsync(ct);
    }

    private static string? NormalizeSlug(string slug)
        => string.IsNullOrWhiteSpace(slug) ? null : slug.Trim().ToLowerInvariant();

    private static string? NormalizeTaxNumber(string taxNumber)
        => string.IsNullOrWhiteSpace(taxNumber) ? null : taxNumber.Trim();
}
