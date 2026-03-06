using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Catalog.Products;

public sealed class ProductImage : Entity<Guid>
{
    public ProductId ProductId { get; private set; } = default!;

    public string Url { get; private set; } = default!;
    public string StorageKey { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }

    public bool IsMain { get; private set; }
    public int SortOrder { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private ProductImage() { } // EF

    private ProductImage(
        Guid id,
        ProductId productId,
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        bool isMain,
        int sortOrder,
        DateTimeOffset createdAt) : base(id)
    {
        ProductId = productId;
        Url = url;
        StorageKey = storageKey;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        IsMain = isMain;
        SortOrder = sortOrder;
        CreatedAt = createdAt;
    }

    internal static ProductImage Create(
        ProductId productId,
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        bool isMain,
        int sortOrder,
        DateTimeOffset nowUtc)
    {
        return new ProductImage(
            id: Guid.NewGuid(),
            productId: productId,
            url: url,
            storageKey: storageKey,
            contentType: contentType,
            sizeBytes: sizeBytes,
            isMain: isMain,
            sortOrder: sortOrder,
            createdAt: nowUtc
        );
    }

    internal void SetMain(bool value) => IsMain = value;

    internal void SetSortOrder(int value) => SortOrder = value;
}