using BazaR.Backend.Application.Carts.DTOs;
using BazaR.Backend.Application.Carts.Queries.GetById;
using BazaR.Backend.Application.Carts.Queries.Search;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/carts")]
[Authorize(Roles = "Admin")]
public sealed class AdminCartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

   
    [HttpGet("{cartId:guid}")]
    public async Task<IActionResult> GetById(Guid cartId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCartByIdQuery(cartId), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        if (result.Value is null)
            return NotFound();

        return Ok(result.Value);
    }

  
    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] Guid? userId,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var filter = new CartFilter
        {
            UserId = userId,
            Status = status
        };

        var pagination = new Pagination(page, pageSize);

        var result = await _mediator.Send(
            new SearchCartsQuery(filter, pagination),
            ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}