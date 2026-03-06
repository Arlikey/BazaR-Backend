namespace BazaR.Backend.Application.Abstractions.Files;

public sealed record FileSaveRequest(
    Stream Content,
    string OriginalFileName,
    string ContentType,
    string Folder,           
    string? DesiredFileName = null 
);