using BazaR.Backend.Application.Orders.Queries.GetMyOrderById;
using BazaR.Backend.Application.Orders.Queries.GetMyOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/orders")]
[Authorize(Roles = "Customer")]
public sealed class CustomerOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerOrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] string? query,
        [FromQuery] string? tab,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetMyOrdersQuery(query, tab, page, pageSize),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetMyOrderById(
        Guid orderId,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMyOrderByIdQuery(orderId), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(result.Value);
    }
}