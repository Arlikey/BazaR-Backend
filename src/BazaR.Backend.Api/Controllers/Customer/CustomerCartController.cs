using BazaR.Backend.Api.Contracts.Carts;
using BazaR.Backend.Application.Carts.Commands.AddItem;
using BazaR.Backend.Application.Carts.Commands.Clear;
using BazaR.Backend.Application.Carts.Commands.RemoveItem;
using BazaR.Backend.Application.Carts.Commands.UpdateItemQuantity;
using BazaR.Backend.Application.Carts.Queries.GetMyActiveCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/me/cart")]
[Authorize]
public sealed class CustomerCartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerCartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyActiveCart(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyActiveCartQuery(), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new AddCartItemCommand(
                OfferId: request.OfferId,
                Quantity: request.Quantity),
            ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    
    [HttpPut("items/{offerId:guid}")]
    public async Task<IActionResult> UpdateItemQuantity(
        Guid offerId,
        [FromBody] UpdateCartItemQuantityRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateCartItemQuantityCommand(
                OfferId: offerId,
                Quantity: request.Quantity),
            ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    
    [HttpDelete("items/{offerId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid offerId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RemoveCartItemCommand(offerId),
            ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

 
    [HttpDelete("items")]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var result = await _mediator.Send(new ClearCartCommand(), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}
