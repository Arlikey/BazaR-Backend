using BazaR.Backend.Application.Abstractions.Integrations.NovaPoshta;
using BazaR.Backend.Application.Shippings.DTOs.NovaPoshta;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchCities;

public sealed class SearchNovaPoshtaCitiesQueryHandler
    : IRequestHandler<SearchNovaPoshtaCitiesQuery, Result<IReadOnlyCollection<NovaPoshtaCityDto>>>
{
    private readonly INovaPoshtaGateway _novaPoshta;

    public SearchNovaPoshtaCitiesQueryHandler(INovaPoshtaGateway novaPoshta)
    {
        _novaPoshta = novaPoshta;
    }

    public async Task<Result<IReadOnlyCollection<NovaPoshtaCityDto>>> Handle(
        SearchNovaPoshtaCitiesQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return Result<IReadOnlyCollection<NovaPoshtaCityDto>>.Failure(
                new Error("NovaPoshta.CitySearch.Query.Required", "Search query is required."));
        }

        var cities = await _novaPoshta.SearchCitiesAsync(request.Query, ct);
        return Result<IReadOnlyCollection<NovaPoshtaCityDto>>.Success(cities);
    }
}