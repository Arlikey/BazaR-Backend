using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Users;

public sealed class UserAvatar : Entity<Guid>
{
    public UserId UserId { get; private set; } = default!;

    public string Url { get; private set; } = default!;
    public string StorageKey { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long SizeBytes { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private UserAvatar() { } // EF

    private UserAvatar(
        Guid id,
        UserId userId,
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        DateTimeOffset createdAt) : base(id)
    {
        UserId = userId;
        Url = url;
        StorageKey = storageKey;
        ContentType = contentType;
        SizeBytes = sizeBytes;
        CreatedAt = createdAt;
    }

    internal static Result<UserAvatar> Create(
        UserId userId,
        string url,
        string storageKey,
        string contentType,
        long sizeBytes,
        DateTimeOffset nowUtc)
    {
        if (userId.Value == default)
            return Result<UserAvatar>.Failure(new Error("UserAvatar.UserRequired", "User required."));

        if (string.IsNullOrWhiteSpace(url))
            return Result<UserAvatar>.Failure(new Error("UserAvatar.UrlRequired", "Avatar url required."));

        if (string.IsNullOrWhiteSpace(storageKey))
            return Result<UserAvatar>.Failure(new Error("UserAvatar.StorageKeyRequired", "Avatar storage key required."));

        if (string.IsNullOrWhiteSpace(contentType))
            return Result<UserAvatar>.Failure(new Error("UserAvatar.ContentTypeRequired", "ContentType required."));

        if (sizeBytes <= 0)
            return Result<UserAvatar>.Failure(new Error("UserAvatar.InvalidSize", "Invalid file size."));

        return Result<UserAvatar>.Success(new UserAvatar(
            id: Guid.NewGuid(),
            userId: userId,
            url: url.Trim(),
            storageKey: storageKey.Trim(),
            contentType: contentType.Trim(),
            sizeBytes: sizeBytes,
            createdAt: nowUtc));
    }
}