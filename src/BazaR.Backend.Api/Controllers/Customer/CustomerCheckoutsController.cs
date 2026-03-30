using BazaR.Backend.Api.Contracts.Customer;
using BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLinePayment;
using BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLineRecipient;
using BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLineShipping;
using BazaR.Backend.Application.Checkouts.Commands.StartCheckout;
using BazaR.Backend.Application.Checkouts.Commands.SubmitCheckout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/checkouts")]
[Authorize]
public sealed class CustomerCheckoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerCheckoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Start(
        [FromBody] StartCheckoutRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new StartCheckoutCommand(), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: "Checkout start failed",
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(new StartCheckoutResponse(result.Value));
    }

    [HttpPut("{checkoutId:guid}/lines/recipient")]
    public async Task<IActionResult> SetLineRecipient(
        Guid checkoutId,
        [FromBody] SetCheckoutLineRecipientRequest request,
        CancellationToken ct)
    {
        var command = new SetCheckoutLineRecipientCommand(
            checkoutId,
            request.LineId,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Email,
            request.IsCustomerRecipient);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPut("{checkoutId:guid}/lines/{lineId:guid}/shipping")]
    public async Task<IActionResult> SetLineShipping(
        Guid checkoutId,
        Guid lineId,
        [FromBody] SetCheckoutLineShippingRequest request,
        CancellationToken ct)
    {
        var command = new SetCheckoutLineShippingCommand(
            checkoutId,
            lineId,
            request.Method,
            request.Country,
            request.Region,
            request.City,
            request.Street,
            request.House,
            request.Apartment,
            request.PostalCode,
            request.WarehouseCode,
            request.WarehouseName,
            request.Comment);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPut("{checkoutId:guid}/lines/{lineId:guid}/payment")]
    public async Task<IActionResult> SetLinePayment(
        Guid checkoutId,
        Guid lineId,
        [FromBody] SetCheckoutLinePaymentRequest request,
        CancellationToken ct)
    {
        var command = new SetCheckoutLinePaymentCommand(
            checkoutId,
            lineId,
            request.Provider,
            request.Method,
            request.RequiresOnlineAuthorization);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("{checkoutId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid checkoutId, CancellationToken ct)
    {
        var command = new SubmitCheckoutCommand(checkoutId);
        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value);
    }
}