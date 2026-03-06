using BazaR.Backend.Application.Abstractions.Files;

namespace BazaR.Backend.Application.Catalog.Products.Services;

public sealed class ProductImageStorage : IProductImageStorage
{
    private readonly IFileStorage _files;

    public ProductImageStorage(IFileStorage files) => _files = files;

    public Task<StoredFile> SaveAsync(
        Guid productId,
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken ct)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("productId is empty.", nameof(productId));

        // единый контракт папок:
        var folder = $"products/{productId:N}";

        var req = new FileSaveRequest(
            Content: content,
            OriginalFileName: originalFileName,
            ContentType: contentType,
            Folder: folder
        );

        return _files.SaveAsync(req, ct);
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
        => _files.DeleteAsync(storageKey, ct);
}