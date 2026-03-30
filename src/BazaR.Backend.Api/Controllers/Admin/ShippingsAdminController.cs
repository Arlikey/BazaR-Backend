using BazaR.Backend.Application.Shippings.Commands.CancelShipping;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingDelivered;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingReadyForPickup;
using BazaR.Backend.Application.Shippings.Queries.GetShippingById;
using BazaR.Backend.Application.Shippings.Queries.GetShippingByOrderId;
using BazaR.Backend.Application.Shippings.Queries.GetShippings;
using BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchCities;
using BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchWarehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/shippings")]
[Authorize(Roles = "Admin")]
public sealed class ShippingsAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShippingsAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetShippings(
        [FromQuery] string? query,
        [FromQuery] int? method,
        [FromQuery] int? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetShippingsQuery(
            query,
            method,
            status,
            page,
            pageSize), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{shippingId:guid}")]
    public async Task<IActionResult> GetById(
        Guid shippingId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetShippingByIdQuery(shippingId), ct);
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId(
        Guid orderId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetShippingByOrderIdQuery(orderId), ct);
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{shippingId:guid}/ready-for-pickup")]
    public async Task<IActionResult> MarkReadyForPickup(
        Guid shippingId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new MarkShippingReadyForPickupCommand(shippingId), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/delivered")]
    public async Task<IActionResult> MarkDelivered(
        Guid shippingId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new MarkShippingDeliveredCommand(shippingId), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid shippingId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new CancelShippingCommand(shippingId), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }
}