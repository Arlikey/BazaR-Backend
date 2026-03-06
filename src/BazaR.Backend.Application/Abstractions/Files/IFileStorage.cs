namespace BazaR.Backend.Application.Abstractions.Files;

public interface IFileStorage
{
    Task<StoredFile> SaveAsync(FileSaveRequest request, CancellationToken ct);
    Task DeleteAsync(string storageKey, CancellationToken ct);
}