using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BazaR.Backend.Application.Abstractions.Files;
using Microsoft.Extensions.Options;

namespace BazaR.Backend.Infrastructure.Persistence.Files;

public sealed class AzureBlobFileStorage : IFileStorage
{
    private readonly AzureBlobStorageOptions _opt;
    private readonly BlobContainerClient _container;

    public AzureBlobFileStorage(IOptions<AzureBlobStorageOptions> opt)
    {
        _opt = opt.Value;

        if (string.IsNullOrWhiteSpace(_opt.ConnectionString))
            throw new InvalidOperationException("BlobStorage:ConnectionString is required.");

        if (string.IsNullOrWhiteSpace(_opt.ContainerName))
            throw new InvalidOperationException("BlobStorage:ContainerName is required.");

        var serviceClient = new BlobServiceClient(_opt.ConnectionString);
        _container = serviceClient.GetBlobContainerClient(_opt.ContainerName);
        _container.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<StoredFile> SaveAsync(FileSaveRequest request, CancellationToken ct)
    {
        if (request.Content is null)
            throw new ArgumentNullException(nameof(request.Content));

        var safeFolder = NormalizeFolder(request.Folder);

        var ext = Path.GetExtension(request.OriginalFileName);
        if (string.IsNullOrWhiteSpace(ext))
            ext = ".bin";

        var fileName = !string.IsNullOrWhiteSpace(request.DesiredFileName)
            ? SanitizeFileName(request.DesiredFileName)
            : $"{Guid.NewGuid():N}{ext}";

        if (!Path.HasExtension(fileName))
            fileName += ext;

        var storageKey = string.IsNullOrWhiteSpace(safeFolder)
            ? fileName
            : $"{safeFolder}/{fileName}";

        var blob = _container.GetBlobClient(storageKey);

        var headers = new BlobHttpHeaders
        {
            ContentType = string.IsNullOrWhiteSpace(request.ContentType)
                ? "application/octet-stream"
                : request.ContentType
        };

        await blob.UploadAsync(
            request.Content,
            new BlobUploadOptions { HttpHeaders = headers },
            ct);

        long sizeBytes = request.Content.CanSeek ? request.Content.Length : 0;

        var url = !string.IsNullOrWhiteSpace(_opt.BaseUrl)
            ? $"{_opt.BaseUrl.TrimEnd('/')}/{storageKey}"
            : blob.Uri.ToString();

        return new StoredFile(
            Url: url,
            StorageKey: storageKey,
            ContentType: headers.ContentType,
            SizeBytes: sizeBytes,
            OriginalFileName: request.OriginalFileName
        );
    }

    public async Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
            return;

        var blob = _container.GetBlobClient(storageKey);
        await blob.DeleteIfExistsAsync(cancellationToken: ct);
    }

    private static string NormalizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return string.Empty;

        folder = folder.Replace("\\", "/").Trim('/');

        if (folder.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("Invalid folder path.");

        return folder;
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (var invalid in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(invalid, '_');

        return fileName;
    }
}