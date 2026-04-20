using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Abstractions.Services;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.GetById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDetailsDto>>
{
    private readonly IProductReadRepository _read;
    private readonly IViewedProductRepository _viewedProducts;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(
        IProductReadRepository read,
        IViewedProductRepository viewedProducts,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _read = read;
        _viewedProducts = viewedProducts;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDetailsDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var dto = await _read.GetByIdAsync(request.ProductId, ct);
        if (dto is null)
            return Result<ProductDetailsDto>.Failure(ProductErrors.NotFound);

        if (_currentUser.UserId != Guid.Empty)
        {
            await _viewedProducts.TrackAsync(_currentUser.UserId, request.ProductId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }

        return Result<ProductDetailsDto>.Success(dto);
    }
}