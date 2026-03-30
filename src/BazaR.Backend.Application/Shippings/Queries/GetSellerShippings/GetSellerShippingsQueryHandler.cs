using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Common;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.GetSellerShippings;

public sealed class GetSellerShippingsQueryHandler
    : IRequestHandler<GetSellerShippingsQuery, Result<PagedResult<ShippingListItemDto>>>
{
    private readonly IShippingReadRepository _readRepository;

    public GetSellerShippingsQueryHandler(IShippingReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<Result<PagedResult<ShippingListItemDto>>> Handle(
        GetSellerShippingsQuery request,
        CancellationToken ct)
    {
        var filter = new ShippingListFilter(
            request.Query,
            request.Method,
            request.Status,
            request.Page,
            request.PageSize);

        var result = await _readRepository.GetSellerPagedAsync(request.SellerId, filter, ct);
        return Result<PagedResult<ShippingListItemDto>>.Success(result);
    }
}