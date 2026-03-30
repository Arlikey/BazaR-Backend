using BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;
using BazaR.Backend.Application.Shippings.DTOs.NovaPoshta;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchWarehouses;

public sealed class SearchNovaPoshtaWarehousesQueryHandler
    : IRequestHandler<SearchNovaPoshtaWarehousesQuery, Result<IReadOnlyCollection<NovaPoshtaWarehouseDto>>>
{
    private readonly INovaPoshtaGateway _novaPoshta;

    public SearchNovaPoshtaWarehousesQueryHandler(INovaPoshtaGateway novaPoshta)
    {
        _novaPoshta = novaPoshta;
    }

    public async Task<Result<IReadOnlyCollection<NovaPoshtaWarehouseDto>>> Handle(
        SearchNovaPoshtaWarehousesQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.CityRef))
        {
            return Result<IReadOnlyCollection<NovaPoshtaWarehouseDto>>.Failure(
                new Error("NovaPoshta.WarehouseSearch.CityRef.Required", "CityRef is required."));
        }

        var warehouses = await _novaPoshta.SearchWarehousesAsync(request.CityRef, request.Query, ct);
        return Result<IReadOnlyCollection<NovaPoshtaWarehouseDto>>.Success(warehouses);
    }
}