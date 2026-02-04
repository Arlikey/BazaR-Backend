using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.GetAttributesView;

public sealed class GetProductAttributesViewQueryHandler
    : IRequestHandler<GetProductAttributesViewQuery, Result<ProductAttributesViewDto>>
{
    private readonly IProductAttributesReadService _service;

    public GetProductAttributesViewQueryHandler(IProductAttributesReadService service) => _service = service;

    public async Task<Result<ProductAttributesViewDto>> Handle(GetProductAttributesViewQuery request, CancellationToken ct)
    {
        var dto = await _service.GetAttributesViewAsync(request.ProductId, ct);
        if (dto is null)
            return Result<ProductAttributesViewDto>.Failure(ProductErrors.NotFound);

        return Result<ProductAttributesViewDto>.Success(dto);
    }
}
