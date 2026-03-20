using BazaR.Backend.Api.Contracts.Seller.Shippings;
using BazaR.Backend.Application.Shippings.Commands.CancelShipping;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingDelivered;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingPickedUp;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingPreparing;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingReadyToShip;
using BazaR.Backend.Application.Shippings.Commands.MarkShippingReturned;
using BazaR.Backend.Application.Shippings.Commands.ShipShipping;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Seller;

[ApiController]
[Route("api/seller/shippings")]
[Authorize]
public sealed class SellerShippingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SellerShippingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{shippingId:guid}/preparing")]
    public async Task<IActionResult> MarkPreparing(
        Guid shippingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkShippingPreparingCommand(shippingId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/ready")]
    public async Task<IActionResult> MarkReadyToShip(
        Guid shippingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkShippingReadyToShipCommand(shippingId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/ship")]
    public async Task<IActionResult> Ship(
        Guid shippingId,
        [FromBody] ShipShippingRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ShipShippingCommand(
                shippingId,
                request.Carrier,
                request.TrackingNumber,
                request.TrackingUrl,
                request.ExternalShipmentId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/delivered")]
    public async Task<IActionResult> MarkDelivered(
        Guid shippingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkShippingDeliveredCommand(shippingId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/picked-up")]
    public async Task<IActionResult> MarkPickedUp(
        Guid shippingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkShippingPickedUpCommand(shippingId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid shippingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new CancelShippingCommand(shippingId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/return")]
    public async Task<IActionResult> MarkReturned(
        Guid shippingId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new MarkShippingReturnedCommand(shippingId),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }
}