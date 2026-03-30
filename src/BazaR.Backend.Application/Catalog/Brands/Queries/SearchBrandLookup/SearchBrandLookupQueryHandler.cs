using BazaR.Backend.Application.Abstractions.ReadModels;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.SearchBrandLookup;

public sealed class SearchBrandLookupQueryHandler
    : IRequestHandler<SearchBrandLookupQuery, IReadOnlyList<BrandLookupDto>>
{
    private readonly IBrandReadRepository _brands;

    public SearchBrandLookupQueryHandler(IBrandReadRepository brands)
    {
        _brands = brands;
    }

    public Task<IReadOnlyList<BrandLookupDto>> Handle(
        SearchBrandLookupQuery request,
        CancellationToken ct)
    {
        var limit = request.Limit <= 0 ? 20 : request.Limit;
        if (limit > 100)
            limit = 100;

        return _brands.SearchActiveAsync(request.Search, limit, ct);
    }
}