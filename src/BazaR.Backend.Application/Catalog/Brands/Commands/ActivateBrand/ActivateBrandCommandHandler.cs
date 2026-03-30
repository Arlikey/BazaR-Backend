using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.ActivateBrand;

public sealed class ActivateBrandCommandHandler : IRequestHandler<ActivateBrandCommand, Result>
{
    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _uow;

    public ActivateBrandCommandHandler(
        IBrandRepository brands,
        IUnitOfWork uow)
    {
        _brands = brands;
        _uow = uow;
    }

    public async Task<Result> Handle(ActivateBrandCommand request, CancellationToken ct)
    {
        var brand = await _brands.GetByIdAsync(new BrandId(request.BrandId), ct);
        if (brand is null)
        {
            return Result.Failure(new Error(
                "Brand.NotFound",
                "Brand was not found."));
        }

        var result = brand.Activate();
        if (result.IsFailure)
            return result;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}