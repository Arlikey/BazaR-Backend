using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Application.Catalog.Products.Queries.GetAttributesView;
using BazaR.Backend.Application.Catalog.Products.Queries.GetById;
using BazaR.Backend.Application.Catalog.Products.Queries.ListByCategory;
using BazaR.Backend.Application.Catalog.Products.Queries.ListBySeller;
using BazaR.Backend.Application.Catalog.Products.Queries.Search;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/catalog/products")]
[AllowAnonymous]
public sealed class PublicProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PublicProductsController(IMediator mediator) => _mediator = mediator;

   
    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> ListByCategory(
        [FromRoute] Guid categoryId,
        [FromQuery] ProductStatus? status,
        CancellationToken ct)
    {
        var isAdmin = User?.Identity?.IsAuthenticated == true && User.IsInRole("Admin");
        var effectiveStatus = isAdmin ? status : ProductStatus.Published;

        var result = await _mediator.Send(
            new ListProductsByCategoryQuery(new(categoryId), effectiveStatus),
            ct);

        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }


    [HttpGet("seller/{sellerId:guid}")]
    public async Task<IActionResult> ListBySeller(
        Guid sellerId,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        var query = new ListProductsBySellerQuery(
            SellerId: sellerId,
            Limit: limit
        );

        var result = await _mediator.Send(query, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid productId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(new(productId)), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

    [HttpGet("{productId:guid}/attributes-view")]
    public async Task<IActionResult> GetAttributesView([FromRoute] Guid productId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProductAttributesViewQuery(new(productId)), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

   
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] ProductStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var pagination = new Pagination(page, pageSize);

        var isAdmin = User?.Identity?.IsAuthenticated == true && User.IsInRole("Admin");

        var effectiveStatus = isAdmin
            ? status ?? ProductStatus.Published
            : ProductStatus.Published;

        var filter = new ProductSearchFilter(q, effectiveStatus);

        var result = await _mediator.Send(
            new SearchProductsQuery(filter, pagination),
            ct);

        if (result.IsFailure)
            return Problem(title: result.Error.Code, detail: result.Error.Message);

        return Ok(result.Value);
    }

    private static int MapStatus(string code) => code switch
    {
        "Product.NotFound" => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}
