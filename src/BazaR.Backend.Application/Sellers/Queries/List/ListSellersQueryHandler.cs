using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Queries.List;

public sealed class ListSellersQueryHandler : IRequestHandler<ListSellersQuery, Result<ListSellersResponse>>
{
    private readonly ISellerReadRepository _sellers;   
    private readonly ICurrentUser _current;

    public ListSellersQueryHandler(ISellerReadRepository sellers, ICurrentUser current)
    {
        _sellers = sellers;
        _current = current;
    }

    public async Task<Result<ListSellersResponse>> Handle(ListSellersQuery request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<ListSellersResponse>.Failure(new Error("Auth.Required", "Authentication required."));

        if (!_current.IsAdmin)
            return Result<ListSellersResponse>.Failure(new Error("Auth.Forbidden", "Admin only."));

        SellerStatus? status = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<SellerStatus>(request.Status, ignoreCase: true, out var parsed))
                return Result<ListSellersResponse>.Failure(new Error("Validation.InvalidStatus", "Invalid status value."));

            status = parsed;
        }

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 200 ? 50 : request.PageSize;

        var (items, total) = await _sellers.ListAsync(status, page, pageSize, ct);

      
        var dto = items.Select(s => new SellerListItemDto(
            Id: s.Id,
            Name: s.Name,
            Slug: s.Slug,
            Type: s.Type,
            Status: s.Status,
            OwnerUserId: s.OwnerUserId,
            CreatedAt: s.CreatedAt
        )).ToList();

        return Result<ListSellersResponse>.Success(new ListSellersResponse(dto, page, pageSize, total));
    }
}
