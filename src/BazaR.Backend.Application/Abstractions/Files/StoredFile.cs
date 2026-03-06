namespace BazaR.Backend.Application.Abstractions.Files;

public sealed record StoredFile(
    string Url,               // публичный URL (пойдёт в домен/DTO)
    string StorageKey,        // ключ/путь в хранилище (для удаления)
    string ContentType,
    long SizeBytes,
    string OriginalFileName
);