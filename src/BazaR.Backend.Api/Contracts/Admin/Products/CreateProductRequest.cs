namespace BazaR.Backend.Api.Contracts.Admin.Products;

public sealed record CreateProductRequest
{
    public string Name { get; init; } = default!;
    public Guid CategoryId { get; init; }
    public Guid? BrandId { get; init; }
    public string? Description { get; init; }
    public string? VendorCode { get; init; }
    public string? Slug { get; init; }

    public IReadOnlyList<CreateProductAttributeRequest>? Attributes { get; init; }
}

public sealed record CreateProductAttributeRequest
{
    public Guid AttributeId { get; init; }

    public string? TextValue { get; init; }
    public decimal? NumberValue { get; init; }
    public bool? BoolValue { get; init; }

    public Guid? OptionId { get; init; }
    public IReadOnlyCollection<Guid>? OptionIds { get; init; }
}
