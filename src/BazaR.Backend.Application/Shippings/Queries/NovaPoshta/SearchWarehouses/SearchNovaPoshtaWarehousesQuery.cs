using BazaR.Backend.Application.Shippings.DTOs.NovaPoshta;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchWarehouses;

public sealed record SearchNovaPoshtaWarehousesQuery(
    string CityRef,
    string? Query) : IRequest<Result<IReadOnlyCollection<NovaPoshtaWarehouseDto>>>;