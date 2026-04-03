using BazaR.Backend.Application.Catalog.Browsing.Queries.GetCategorySidebar;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Catalog;

[ApiController]
[Route("api/catalog/categories")]
public sealed class CatalogSidebarController : ControllerBase
{
    private readonly ISender _sender;

    public CatalogSidebarController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{categoryId:guid}/sidebar")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSidebar(
        [FromRoute] Guid categoryId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new GetCategorySidebarQuery(categoryId), ct);
        return Ok(result);
    }
}