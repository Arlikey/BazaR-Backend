namespace BazaR.Backend.Infrastructure.Persistence.Files;

public sealed class AzureBlobStorageOptions
{
    public string ConnectionString { get; set; } = default!;
    public string ContainerName { get; set; } = "media";
    public string? BaseUrl { get; set; }
}