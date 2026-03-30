using BazaR.Backend.Application.Abstractions.ReadModels;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.GetBrandsLookup;

public sealed record GetBrandsLookupQuery(
    int Limit) : IRequest<IReadOnlyList<BrandLookupDto>>;