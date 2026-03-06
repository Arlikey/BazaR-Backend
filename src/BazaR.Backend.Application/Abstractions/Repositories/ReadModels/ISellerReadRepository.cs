
using BazaR.Backend.Domain.Sellers;

namespace BazaR.Backend.Application.Abstractions.Repositories;

public interface ISellerReadRepository
{
    Task<SellerListItem?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<(IReadOnlyList<SellerListItem> Items, long Total)> ListAsync(
        SellerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<SellerListItem> Items, long Total)> ListPendingApprovalAsync(
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<(IReadOnlyList<SellerListItem> Items, long Total)> SearchAsync(
        string? query,
        SellerStatus? status,
        int page,
        int pageSize,
        CancellationToken ct = default);
}
