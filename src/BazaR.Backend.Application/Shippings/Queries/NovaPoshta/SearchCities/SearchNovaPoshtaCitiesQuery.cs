using BazaR.Backend.Application.Shippings.DTOs.NovaPoshta;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchCities;

public sealed record SearchNovaPoshtaCitiesQuery(
    string Query) : IRequest<Result<IReadOnlyCollection<NovaPoshtaCityDto>>>;