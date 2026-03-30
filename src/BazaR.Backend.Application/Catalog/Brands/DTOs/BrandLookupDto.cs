namespace BazaR.Backend.Application.Abstractions.ReadModels;

public sealed class BrandLookupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? LogoUrl { get; init; }
}