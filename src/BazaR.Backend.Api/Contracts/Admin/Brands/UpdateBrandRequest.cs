namespace BazaR.Backend.Api.Contracts.Admin.Brands;

public sealed class UpdateBrandRequest
{
    public string Name { get; init; } = default!;
    public string Slug { get; init; } = default!;
    public string? LogoUrl { get; init; }
    public string? Description { get; init; }
}