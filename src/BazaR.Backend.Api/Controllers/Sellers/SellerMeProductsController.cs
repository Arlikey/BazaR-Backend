

//using BazaR.Backend.Api.Contracts.Admin.Products;
using BazaR.Backend.Api.Contracts.Sellers;
using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Catalog.Products.Commands.AddImages;
using BazaR.Backend.Application.Catalog.Products.Commands.CreateProduct;
using BazaR.Backend.Application.Catalog.Products.Queries.GetAttributesView;
using BazaR.Backend.Application.Catalog.Products.Queries.GetById;

using BazaR.Backend.Application.Catalog.Products.Queries.ListBySeller;

/*using BazaR.Backend.Application.Catalog.Products.Queries.ListMy;
using BazaR.Backend.Application.Catalog.Products.Queries.ListMyByCategory;*/
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;




namespace BazaR.Backend.Api.Controllers.Sellers;

[ApiController]
[Route("api/seller/me/products")]
[Authorize(Roles = "Seller")]
public sealed class SellerMeProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SellerMeProductsController(IMediator mediator) => _mediator = mediator;

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var attrs = (request.Attributes ?? Array.Empty<CreateProductAttributeRequest>())
            .Select(a => new ProductAttributeInput(
                AttributeId: a.AttributeId,
                TextValue: a.TextValue,
                NumberValue: a.NumberValue,
                BoolValue: a.BoolValue,
                OptionId: a.OptionId,
                OptionIds: a.OptionIds
            ))
            .ToList();

        var command = new CreateProductCommand(
            Name: request.Name,
            CategoryId: new CategoryId(request.CategoryId),
            Attributes: attrs,
            Description: request.Description,
            BrandId: request.BrandId,
            VendorCode: request.VendorCode,
            Slug: request.Slug,
            Barcode: request.Barcode
        );

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure) return ProblemFromError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Value },
            new CreateProductResponse { ProductId = result.Value.Value });
    }


    //  ADD IMAGES (multipart)
    [HttpPost("{id:guid}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddImages(
        Guid id,
        [FromForm] List<IFormFile> files,
        [FromForm] bool makeFirstImageMain = true,
        CancellationToken ct = default)
    {
        if (files is null || files.Count == 0)
            return BadRequest("No files provided.");

        var uploads = new List<UploadFile>(files.Count);

        foreach (var file in files)
        {
            if (file.Length == 0)
                return BadRequest($"File {file.FileName} is empty.");

            uploads.Add(new UploadFile(
                Content: file.OpenReadStream(),
                FileName: file.FileName,
                ContentType: file.ContentType,
                SizeBytes: file.Length
            ));
        }

        var command = new AddProductImagesCommand(
            ProductId: new ProductId(id),
            Images: uploads,
            MakeFirstImageMain: makeFirstImageMain
        );

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(new
        {
            ImageIds = result.Value
        });
    }

   
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(new ProductId(id)), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

   

    private static int MapStatus(string code) => code switch
    {
        "Auth.Required" => StatusCodes.Status401Unauthorized,

        "Seller.NotFound" => StatusCodes.Status404NotFound,
        "Seller.NotActive" => StatusCodes.Status409Conflict,

        "Category.NotFound" => StatusCodes.Status404NotFound,
        "Attribute.NotFound" => StatusCodes.Status404NotFound,

        "Product.SlugAlreadyExists" => StatusCodes.Status409Conflict,
        "Product.VendorCodeAlreadyExists" => StatusCodes.Status409Conflict,

        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}
