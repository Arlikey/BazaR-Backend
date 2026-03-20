using BazaR.Backend.Application.Favorites.Commands.AddProductToFavorites;
using BazaR.Backend.Application.Favorites.Commands.RemoveProductFromFavorites;
using BazaR.Backend.Application.Favorites.Queries.GetMyFavorites;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/favorites")]
[Authorize]
public sealed class CustomerFavoritesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerFavoritesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================
    // GET my favorites
    // =========================
    [HttpGet]
    public async Task<IActionResult> GetMyFavorites(
        [FromQuery] int limit = 20,
        CancellationToken ct = default)
    {
        var query = new GetMyFavoritesQuery(limit);

        var result = await _mediator.Send(query, ct);

        return Ok(result);
    }

    // =========================
    // ADD to favorites
    // =========================
    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Add(
        Guid productId,
        CancellationToken ct)
    {
        var cmd = new AddProductToFavoritesCommand(productId);

        var result = await _mediator.Send(cmd, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    // =========================
    // REMOVE from favorites
    // =========================
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Remove(
        Guid productId,
        CancellationToken ct)
    {
        var cmd = new RemoveProductFromFavoritesCommand(productId);

        var result = await _mediator.Send(cmd, ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}