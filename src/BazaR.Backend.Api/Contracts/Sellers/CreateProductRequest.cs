namespace BazaR.Backend.Api.Contracts.Sellers;

public sealed record CreateProductRequest(
    string Name,
    Guid CategoryId,
    string? Description = null,
    Guid? BrandId = null,
    string? VendorCode = null,
    string? Slug = null,
    string? Barcode = null,
    IReadOnlyList<CreateProductAttributeRequest>? Attributes = null);

public sealed record CreateProductAttributeRequest(
    Guid AttributeId,
    string? TextValue = null,
    decimal? NumberValue = null,
    bool? BoolValue = null,
    Guid? OptionId = null,
    IReadOnlyCollection<Guid>? OptionIds = null);

public sealed record CreateProductResponse
{
    public Guid ProductId { get; init; }
}



