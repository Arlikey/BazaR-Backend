using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.GetById;

public sealed class GetMyProductByIdQueryHandler
    : IRequestHandler<GetMyProductByIdQuery, Result<ProductDetailsDto>>
{
    private readonly IProductReadRepository _read;

    public GetMyProductByIdQueryHandler(IProductReadRepository read) => _read = read;

    public async Task<Result<ProductDetailsDto>> Handle(GetMyProductByIdQuery request, CancellationToken ct)
    {
        // Нужен метод с проверкой владельца
        var dto = await _read.GetByIdForOwnerAsync(request.ProductId, request.SellerId, ct);

        if (dto is null)
            return Result<ProductDetailsDto>.Failure(ProductErrors.NotFound);
        

        return Result<ProductDetailsDto>.Success(dto);
    }
}
