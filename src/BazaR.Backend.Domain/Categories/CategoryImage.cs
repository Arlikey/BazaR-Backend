using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Categories;

public sealed class CategoryImage : Entity<Guid>
{
    private const int MaxUrlLength = 2000;
    private const int MaxStorageKeyLength = 512;
    private const int MaxContentTypeLength = 100;

    public string Url { get; private set; } = default!;
    public string StorageKey { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private CategoryImage(
        Guid id,
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        DateTimeOffset nowUtc) : base(id)
    {
        Url = url;
        StorageKey = storageKey;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        CreatedAt = nowUtc;
    }

    private CategoryImage() { } 

    internal static Result<CategoryImage> Create(
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        DateTimeOffset nowUtc)
    {
        url = (url ?? "").Trim();
        storageKey = (storageKey ?? "").Trim();
        contentType = (contentType ?? "").Trim();

        if (string.IsNullOrWhiteSpace(url))
            return Result<CategoryImage>.Failure(new Error("CategoryImage.UrlRequired", "Image url is required."));
        if (url.Length > MaxUrlLength)
            return Result<CategoryImage>.Failure(new Error("CategoryImage.UrlTooLong", "Image url is too long."));

        if (string.IsNullOrWhiteSpace(storageKey))
            return Result<CategoryImage>.Failure(new Error("CategoryImage.StorageKeyRequired", "Storage key is required."));
        if (storageKey.Length > MaxStorageKeyLength)
            return Result<CategoryImage>.Failure(new Error("CategoryImage.StorageKeyTooLong", "Storage key is too long."));

        if (string.IsNullOrWhiteSpace(contentType))
            return Result<CategoryImage>.Failure(new Error("CategoryImage.ContentTypeRequired", "Content type is required."));
        if (contentType.Length > MaxContentTypeLength)
            return Result<CategoryImage>.Failure(new Error("CategoryImage.ContentTypeTooLong", "Content type is too long."));

        if (sizeBytes <= 0)
            return Result<CategoryImage>.Failure(new Error("CategoryImage.InvalidSize", "Invalid file size."));

        return Result<CategoryImage>.Success(
            new CategoryImage(Guid.NewGuid(), url, storageKey, contentType, sizeBytes, nowUtc));
    }

    internal void Replace(string url, string storageKey, string contentType, long sizeBytes, DateTimeOffset nowUtc)
    {
        Url = url.Trim();
        StorageKey = storageKey.Trim();
        ContentType = contentType.Trim();
        SizeBytes = sizeBytes;
        CreatedAt = nowUtc;
    }
}