/*using BazaR.Backend.Application.Abstractions.Files;
using Microsoft.Extensions.Options;

namespace BazaR.Backend.Infrastructure.Persistence.Files;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly LocalFileStorageOptions _opt;

    public LocalFileStorage(IOptions<LocalFileStorageOptions> opt)
    {
        _opt = opt.Value;

        if (string.IsNullOrWhiteSpace(_opt.BasePath))
            throw new InvalidOperationException("BasePath is required.");

        if (string.IsNullOrWhiteSpace(_opt.RequestPath))
            _opt.RequestPath = "/uploads";
    }

    public async Task<StoredFile> SaveAsync(FileSaveRequest request, CancellationToken ct)
    {
        if (request.Content is null)
            throw new ArgumentNullException(nameof(request.Content));

        var safeFolder = NormalizeFolder(request.Folder);

        var folderPath = Path.Combine(_opt.BasePath, safeFolder);
        Directory.CreateDirectory(folderPath);

        var ext = Path.GetExtension(request.OriginalFileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".bin";

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(folderPath, fileName);

        long sizeBytes;

        await using (var fs = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            true))
        {
            await request.Content.CopyToAsync(fs, ct);
            await fs.FlushAsync(ct);
            sizeBytes = fs.Length;
        }

        var storageKey = $"{safeFolder}/{fileName}".Replace("\\", "/");

        // ВАЖНО: формируем URL через RequestPath
        var url = $"{_opt.RequestPath}/{storageKey}".Replace("\\", "/");

        return new StoredFile(
            Url: url,
            StorageKey: storageKey,
            ContentType: request.ContentType,
            SizeBytes: sizeBytes,
            OriginalFileName: request.OriginalFileName
        );
    }

    public Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            return Task.CompletedTask;

        var safeKey = storageKey.Replace("/", Path.DirectorySeparatorChar.ToString());
        var fullPath = Path.Combine(_opt.BasePath, safeKey);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }

    private static string NormalizeFolder(string folder)
    {
        folder = folder.Replace("\\", "/").Trim('/');

        if (folder.Contains(".."))
            throw new InvalidOperationException("Invalid folder path.");

        return folder;
    }
}*/