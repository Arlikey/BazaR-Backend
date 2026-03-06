using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Users.Commands.SetAvatar;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.SetMyAvatar;

public sealed class SetMyAvatarCommandHandler : IRequestHandler<SetMyAvatarCommand, Result>
{
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; 
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp"
    };

    private readonly IUserRepository _users;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;

    public SetMyAvatarCommandHandler(
        IUserRepository users,
        ICurrentUser current,
        IUnitOfWork uow,
        IFileStorage files)
    {
        _users = users;
        _current = current;
        _uow = uow;
        _files = files;
    }

    public async Task<Result> Handle(SetMyAvatarCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var file = request.Image;
        if (file is null || file.SizeBytes <= 0)
            return Result.Failure(new Error("User.Avatar.Empty", "Avatar is empty."));

        if (file.SizeBytes > MaxFileSizeBytes)
            return Result.Failure(new Error("User.Avatar.TooLarge", $"Max size is {MaxFileSizeBytes} bytes."));

        if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedContentTypes.Contains(file.ContentType))
            return Result.Failure(new Error("User.Avatar.InvalidType", "Allowed: jpeg, png, webp."));

        var userId = new UserId(_current.UserId);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found."));

        var oldKey = user.Avatar?.StorageKey; 
        string? newKey = null;

        try
        {
            // 1) сохраняем файл общим стораджем
            var folder = $"users/{user.Id.Value:N}/avatar";

            var stored = await _files.SaveAsync(new FileSaveRequest(
                Folder: folder,
                Content: file.Content,
                OriginalFileName: file.FileName,
                ContentType: file.ContentType
            ), ct);

            newKey = stored.StorageKey;

            // 2) пишем в домен
            var setRes = user.SetAvatar(
                url: stored.Url,
                storageKey: stored.StorageKey,
                contentType: stored.ContentType,
                sizeBytes: stored.SizeBytes);

            if (setRes.IsFailure)
            {
                try { await _files.DeleteAsync(stored.StorageKey, ct); } catch { }
                return Result.Failure(setRes.Error);
            }

            _users.Update(user);
            await _uow.SaveChangesAsync(ct);

            // 3) удаляем старый после коммита
            if (!string.IsNullOrWhiteSpace(oldKey) && !string.Equals(oldKey, stored.StorageKey, StringComparison.Ordinal))
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