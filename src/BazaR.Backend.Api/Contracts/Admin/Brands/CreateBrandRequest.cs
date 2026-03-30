namespace BazaR.Backend.Api.Contracts.Admin.Brands;

using Microsoft.AspNetCore.Http;

public sealed class CreateBrandRequest
{
    public string Name { get; set; } = default!;
    public string? Slug { get; set; }
    public IFormFile? Logo { get; set; }
    public string? Description { get; set; }
}