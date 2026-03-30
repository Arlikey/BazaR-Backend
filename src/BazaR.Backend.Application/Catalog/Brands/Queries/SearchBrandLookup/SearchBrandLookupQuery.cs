using BazaR.Backend.Application.Abstractions.ReadModels;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.SearchBrandLookup;

public sealed record SearchBrandLookupQuery(
    string? Search,
    int Limit) : IRequest<IReadOnlyList<BrandLookupDto>>;