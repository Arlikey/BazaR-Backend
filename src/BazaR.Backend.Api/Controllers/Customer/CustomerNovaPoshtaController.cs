using BazaR.Backend.Api.Contracts.Customer.NovaPoshta;
using BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchCities;
using BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchWarehouses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/nova-poshta")]
[Authorize]
public sealed class CustomerNovaPoshtaController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerNovaPoshtaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("cities")]
    public async Task<IActionResult> SearchCities(
        [FromQuery] string query,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SearchNovaPoshtaCitiesQuery(query),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var response = result.Value
            .Select(x => new NovaPoshtaCityResponse(
                x.Ref,
                x.Description,
                x.Area,
                x.SettlementType))
            .ToList();

        return Ok(response);
    }

    [HttpGet("warehouses")]
    public async Task<IActionResult> SearchWarehouses(
        [FromQuery] string cityRef,
        [FromQuery] string? query,
        CancellationToken ct)
    {
        var result = await _mediator.Send(
            new SearchNovaPoshtaWarehousesQuery(cityRef, query),
            ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var response = result.Value
            .Select(x => new NovaPoshtaWarehouseResponse(
                x.Ref,
                x.Number,
                x.Description,
                x.CityRef,
                x.CategoryOfWarehouse))
            .ToList();

        return Ok(response);
    }
}


