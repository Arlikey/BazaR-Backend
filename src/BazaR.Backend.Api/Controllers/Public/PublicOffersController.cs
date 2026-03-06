
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/catalog/offers")]
[AllowAnonymous]
public sealed class PublicOffersController : ControllerBase
{
    private readonly IMediator _mediator;
    public PublicOffersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("by-product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOfferByProductIdQuery(productId), ct);
        if (result.IsFailure)
            return BadRequest(result.Error);
        if (result.Value is null)
            return NotFound();
        return Ok(result.Value);
    }
}