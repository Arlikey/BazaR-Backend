using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    CategoryId CategoryId,
    IReadOnlyList<ProductAttributeInput>? Attributes,
    string? Description = null,
    Guid? BrandId = null,
    string? VendorCode = null,
    string? Slug = null,
    string? Barcode = null,

   
    IReadOnlyList<UploadFile>? Images = null,
    bool MakeFirstImageMain = true
) : IRequest<Result<ProductId>>;

public sealed record ProductAttributeInput(
    Guid AttributeId,
    string? TextValue = null,
    decimal? NumberValue = null,
    bool? BoolValue = null,
    Guid? OptionId = null,
    IReadOnlyCollection<Guid>? OptionIds = null
);