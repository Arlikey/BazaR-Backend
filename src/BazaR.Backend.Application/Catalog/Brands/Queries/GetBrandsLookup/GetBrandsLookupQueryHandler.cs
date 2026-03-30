using BazaR.Backend.Application.Abstractions.ReadModels;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.GetBrandsLookup;

public sealed class GetBrandsLookupQueryHandler
    : IRequestHandler<GetBrandsLookupQuery, IReadOnlyList<BrandLookupDto>>
{
    private readonly IBrandReadRepository _brands;

    public GetBrandsLookupQueryHandler(IBrandReadRepository brands)
    {
        _brands = brands;
    }

    public Task<IReadOnlyList<BrandLookupDto>> Handle(
        GetBrandsLookupQuery request,
        CancellationToken ct)
    {
        var limit = request.Limit <= 0 ? 50 : request.Limit;
        if (limit > 200)
            limit = 200;

        // просто вызываем поиск без строки → вернет все active
        return _brands.SearchActiveAsync(null, limit, ct);
    }
}