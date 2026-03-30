using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Brands;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.CreateBrand;

public sealed class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    private const int MaxFileSizeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;

    public CreateBrandCommandHandler(
        IBrandRepository brands,
        IUnitOfWork uow,
        IFileStorage files)
    {
        _brands = brands;
        _uow = uow;
        _files = files;
    }

    public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken ct)
    {
        var normalizedName = request.Name?.Trim();
        var normalizedSlug = request.Slug?.Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(normalizedName))
        {
            var existsByName = await _brands.ExistsByNameAsync(normalizedName, ct);
            if (existsByName)
                return Result<Guid>.Failure(new Error("Brand.Name.AlreadyExists", "Brand already exists."));
        }

        if (!string.IsNullOrWhiteSpace(normalizedSlug))
        {
            var existsBySlug = await _brands.ExistsBySlugAsync(normalizedSlug, ct);
            if (existsBySlug)
                return Result<Guid>.Failure(new Error("Brand.Slug.AlreadyExists", "Slug already exists."));
        }

        string? logoUrl = null;
        string? storageKey = null;

        try
        {
            if (request.Logo is not null)
            {
                var file = request.Logo;

                if (file.SizeBytes <= 0)
                    return Result<Guid>.Failure(new Error("Brand.Logo.Empty", "Logo is empty."));

                if (file.SizeBytes > MaxFileSizeBytes)
                    return Result<Guid>.Failure(new Error("Brand.Logo.TooLarge", "Max 5MB."));

                if (!AllowedContentTypes.Contains(file.ContentType))
                    return Result<Guid>.Failure(new Error("Brand.Logo.InvalidType", "Only jpeg/png/webp."));

                var folder = string.IsNullOrWhiteSpace(normalizedSlug)
                    ? "brands/temp"
                    : $"brands/{normalizedSlug}";

                var stored = await _files.SaveAsync(new FileSaveRequest(
                    Folder: folder,
                    Content: file.Content,
                    OriginalFileName: file.FileName,
                    ContentType: file.ContentType
                ), ct);

                logoUrl = stored.Url;
                storageKey = stored.StorageKey;
            }

            var createResult = Brand.Create(
                request.Name,
                request.Slug,
                logoUrl,
                request.Description);

            if (createResult.IsFailure)
            {
                if (!string.IsNullOrWhiteSpace(storageKey))
                    await _files.DeleteAsync(storageKey, ct);

                return Result<Guid>.Failure(createResult.Error);
            }

            var brand = createResult.Value!;

            await _brands.AddAsync(brand, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<Guid>.Success(brand.Id.Value);
        }
        catch
        {
            if (!string.IsNullOrWhiteSpace(storageKey))
                await _files.DeleteAsync(storageKey, ct);

            throw;
        }
    }
}