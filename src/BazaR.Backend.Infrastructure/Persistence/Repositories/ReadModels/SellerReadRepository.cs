using BazaR.Backend.Application.Abstractions.Repositories;

using BazaR.Backend.Domain.Sellers;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.Repositories;

public sealed class SellerReadRepository : ISellerReadRepository
{
    private readonly AppDbContext _db;

    public SellerReadRepository(AppDbContext db) => _db = db;

    public async Task<SellerListItem?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.Sellers.AsNoTracking()
            .Where(x => x.Id.Value == id)
            .Select(ToListItem())
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(IReadOnlyList<SellerListItem> Items, long Total)> ListAsync(
        SellerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        NormalizePaging(ref page, ref pageSize);

        var q = _db.Sellers.AsNoTracking().AsQueryable();

        if (status is not null)
            q = q.Where(x => x.Status == status);

        var total = await q.LongCountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItem())
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<SellerListItem> Items, long Total)> ListPendingApprovalAsync(
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        NormalizePaging(ref page, ref pageSize);

        var q = _db.Sellers.AsNoTracking()
            .Where(x => x.Status == SellerStatus.PendingApproval);

        var total = await q.LongCountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.SubmittedAt ?? x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItem())
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<SellerListItem> Items, long Total)> SearchAsync(
        string? query,
        SellerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        NormalizePaging(ref page, ref pageSize);

        var q = _db.Sellers.AsNoTracking().AsQueryable();

        if (status is not null)
            q = q.Where(x => x.Status == status);

        var normalized = (query ?? "").Trim();
        if (!string.IsNullOrWhiteSpace(normalized))
        {
            var like = normalized.ToLowerInvariant();

            // простой search: name/slug/tax
            q = q.Where(x =>
                x.Name.ToLower()!.Contains(like) ||
                x.Slug.Value.Contains(like) ||
                (x.TaxNumber != null && x.TaxNumber.Contains(normalized)));
        }

        var total = await q.LongCountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItem())
            .ToListAsync(ct);

        return (items, total);
    }

    private static System.Linq.Expressions.Expression<Func<Seller, SellerListItem>> ToListItem()
        => s => new SellerListItem(
            s.Id.Value,
            s.Name,
            s.Slug.Value,
            s.Type,
            s.Status,
            s.OwnerUserId,
            s.TaxNumber,
            s.CountryCode.Value,
            s.CreatedAt,
            s.UpdatedAt,
            s.SubmittedAt
        );

    private static void NormalizePaging(ref int page, ref int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;
        if (pageSize > 200) pageSize = 200;
    }
}
