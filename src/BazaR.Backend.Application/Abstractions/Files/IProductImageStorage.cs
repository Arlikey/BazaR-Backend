using BazaR.Backend.Application.Abstractions.Files;

namespace BazaR.Backend.Application.Abstractions.Files;

public interface IProductImageStorage
{
    Task<StoredFile> SaveAsync(
        Guid productId,
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken ct);

    Task DeleteAsync(string storageKey, CancellationToken ct);
}