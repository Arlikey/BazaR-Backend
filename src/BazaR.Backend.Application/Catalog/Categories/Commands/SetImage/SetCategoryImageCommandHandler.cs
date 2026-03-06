using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Catalog.Categories.Commands.SetImage;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Categories.Commands.SetImage;

public sealed class SetCategoryImageCommandHandler : IRequestHandler<SetCategoryImageCommand, Result>
{
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;

    public SetCategoryImageCommandHandler(
        ICategoryRepository categories,
        IUnitOfWork uow,
        IFileStorage files)
    {
        _categories = categories;
        _uow = uow;
        _files = files;
    }

    public async Task<Result> Handle(SetCategoryImageCommand request, CancellationToken ct)
    {
        var file = request.Image;
        if (file is null || file.SizeBytes <= 0)
            return Result.Failure(new Error("Category.Image.Empty", "Image is empty."));

        if (file.SizeBytes > MaxFileSizeBytes)
            return Result.Failure(new Error("Category.Image.TooLarge", $"Max size is {MaxFileSizeBytes} bytes."));

        if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedContentTypes.Contains(file.ContentType))
            return Result.Failure(new Error("Category.Image.InvalidType", "Allowed: jpeg, png, webp."));

        var category = await _categories.GetByIdAsync(request.CategoryId, ct);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

     
        var oldKey = category.Image?.StorageKey;
        string? newKey = null;

        try
        {
            
            var folder = $"categories/{category.Id.Value:N}/image";

            var stored = await _files.SaveAsync(new FileSaveRequest(
                Folder: folder,
                Content: file.Content,
                OriginalFileName: file.FileName,
                ContentType: file.ContentType
            ), ct);

            newKey = stored.StorageKey;

            
            var setRes = category.SetImage(
                url: stored.Url,
                storageKey: stored.StorageKey,
                contentType: stored.ContentType,
                sizeBytes: stored.SizeBytes);

            if (setRes.IsFailure)
            {
                try { await _files.DeleteAsync(stored.StorageKey, ct); } catch { }
                return Result.Failure(setRes.Error);
            }

           
            await _uow.SaveChangesAsync(ct);

            
            if (!string.IsNullOrWhiteSpace(oldKey) &&
                !string.Equals(oldKey, stored.StorageKey, StringComparison.Ordinal))
            {
                try { await _files.DeleteAsync(oldKey, ct); } catch { }
            }

            return Result.Success();
        }
        catch
        {
           
            if (!string.IsNullOrWhiteSpace(newKey))
            {
                try { await _files.DeleteAsync(newKey, ct); } catch { }
            }
            throw;
        }
    }
}