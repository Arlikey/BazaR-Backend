using BazaR.Backend.Application.Abstractions.ReadModels;

using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.GetBrandById;

public sealed class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, BrandDetailsDto?>
{
    private readonly IBrandReadRepository _brands;

    public GetBrandByIdQueryHandler(IBrandReadRepository brands)
    {
        _brands = brands;
    }

    public Task<BrandDetailsDto?> Handle(GetBrandByIdQuery request, CancellationToken ct)
    {
        return _brands.GetByIdAsync(request.BrandId, ct);
    }
}