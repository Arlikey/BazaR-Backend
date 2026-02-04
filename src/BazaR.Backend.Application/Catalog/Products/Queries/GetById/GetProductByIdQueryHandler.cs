using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.GetById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDetailsDto>>
{
    private readonly IProductReadRepository _read;

    public GetProductByIdQueryHandler(IProductReadRepository read) => _read = read;

    public async Task<Result<ProductDetailsDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var dto = await _read.GetByIdAsync(request.ProductId, ct);
        if (dto is null)
            return Result<ProductDetailsDto>.Failure(ProductErrors.NotFound);

        return Result<ProductDetailsDto>.Success(dto);
    }
}
