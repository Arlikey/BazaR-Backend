using BazaR.Backend.Application.Catalog.Brands.Queries.GetBrandsLookup;
using BazaR.Backend.Application.Catalog.Brands.Queries.SearchBrandLookup;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Catalog;

[ApiController]
[Route("api/catalog/brands")]
[Authorize(Roles = "Seller")]
public sealed class BrandsController : ControllerBase
{
    private readonly ISender _sender;

    public BrandsController(ISender sender)
    {
        _sender = sender;
    }

 
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetList(
        [FromQuery] int limit = 50,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetBrandsLookupQuery(limit), ct);
        return Ok(result);
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery(Name = "q")] string? search,
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new SearchBrandLookupQuery(search, limit), ct);
        return Ok(result);
    }
}