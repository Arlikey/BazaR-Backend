using BazaR.Backend.Api.Contracts.Payments;
using BazaR.Backend.Application.Payments.Commands.ProcessLiqPayCallback;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Webhooks;

[ApiController]
[Route("api/webhooks/liqpay")]
[AllowAnonymous]
public sealed class LiqPayWebhookController : ControllerBase
{
    private readonly IMediator _mediator;

    public LiqPayWebhookController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
    public async Task<IActionResult> HandleCallback(
        [FromForm] LiqPayCallbackRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ProcessLiqPayCallbackCommand(request.Data, request.Signature), ct);

        if (result.IsFailure)
        {
            return BadRequest(new LiqPayWebhookErrorResponse(
                result.Error.Code,
                result.Error.Message));
        }

        return Ok(new LiqPayWebhookOkResponse("ok"));
    }
} 