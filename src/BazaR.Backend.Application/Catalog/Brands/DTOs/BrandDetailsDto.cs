namespace BazaR.Backend.Application.Abstractions.ReadModels;

public sealed class BrandDetailsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? LogoUrl { get; init; }
    public string? Description { get; init; }
    public string Status { get; init; } = default!;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}