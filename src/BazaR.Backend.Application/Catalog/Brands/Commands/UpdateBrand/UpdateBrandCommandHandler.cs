using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.UpdateBrand;

public sealed class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result>
{
    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _uow;

    public UpdateBrandCommandHandler(
        IBrandRepository brands,
        IUnitOfWork uow)
    {
        _brands = brands;
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken ct)
    {
        var brand = await _brands.GetByIdAsync(new BrandId(request.BrandId), ct);
        if (brand is null)
        {
            return Result.Failure(new Error(
                "Brand.NotFound",
                "Brand was not found."));
        }

        var normalizedName = request.Name.Trim();
        if (!string.Equals(brand.Name, normalizedName, StringComparison.Ordinal))
        {
            var existsByName = await _brands.ExistsByNameAsync(normalizedName, ct);
            if (existsByName)
            {
                return Result.Failure(new Error(
                    "Brand.Name.AlreadyExists",
                    "Brand with the same name already exists."));
            }
        }

        var normalizedSlug = request.Slug.Trim().ToLowerInvariant();
        if (!string.Equals(brand.Slug, normalizedSlug, StringComparison.Ordinal))
        {
            var existsBySlug = await _brands.ExistsBySlugAsync(normalizedSlug, ct);
            if (existsBySlug)
            {
                return Result.Failure(new Error(
                    "Brand.Slug.AlreadyExists",
                    "Brand with the same slug already exists."));
            }
        }

        var renameResult = brand.Rename(request.Name);
        if (renameResult.IsFailure)
            return renameResult;

        var slugResult = brand.ChangeSlug(request.Slug);
        if (slugResult.IsFailure)
            return slugResult;

        var logoResult = brand.ChangeLogo(request.LogoUrl);
        if (logoResult.IsFailure)
            return logoResult;

        var descriptionResult = brand.ChangeDescription(request.Description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}