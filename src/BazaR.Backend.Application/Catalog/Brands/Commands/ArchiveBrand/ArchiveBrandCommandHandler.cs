using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.ArchiveBrand;

public sealed class ArchiveBrandCommandHandler : IRequestHandler<ArchiveBrandCommand, Result>
{
    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _uow;

    public ArchiveBrandCommandHandler(
        IBrandRepository brands,
        IUnitOfWork uow)
    {
        _brands = brands;
        _uow = uow;
    }

    public async Task<Result> Handle(ArchiveBrandCommand request, CancellationToken ct)
    {
        var brand = await _brands.GetByIdAsync(new BrandId(request.BrandId), ct);
        if (brand is null)
        {
            return Result.Failure(new Error(
                "Brand.NotFound",
                "Brand was not found."));
        }

        var result = brand.Archive();
        if (result.IsFailure)
            return result;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}