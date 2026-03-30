using BazaR.Backend.Api.Contracts.Shippings;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Shippings.Commands.DispatchShipping;
using BazaR.Backend.Application.Shippings.Commands.SetShippingParcels;
using BazaR.Backend.Application.Shippings.Commands.SetShippingSender;
using BazaR.Backend.Application.Shippings.Queries.GetSellerShippings;
using BazaR.Backend.Application.Shippings.Queries.GetShippingById;
using BazaR.Backend.Application.Shippings.Queries.GetShippingByOrderId;
using BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchCities;
using BazaR.Backend.Application.Shippings.Queries.NovaPoshta.SearchWarehouses;
using BazaR.Backend.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Seller;

[ApiController]
[Route("api/seller/shippings")]
[Authorize(Roles = "Seller")]
public sealed class SellerShippingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;
    private readonly ISellerRepository _sellers;

    public SellerShippingsController(
        IMediator mediator,
        ICurrentUser currentUser,
        ISellerRepository sellers)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _sellers = sellers;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyShippings(
        [FromQuery] string? query,
        [FromQuery] int? method,
        [FromQuery] int? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var result = await _mediator.Send(new GetSellerShippingsQuery(
            seller.Id.Value,
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
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var result = await _mediator.Send(new GetShippingByIdQuery(shippingId), ct);
        if (result.IsFailure)
            return NotFound(result.Error);

        if (result.Value!.SellerId != seller.Id.Value)
            return Forbid();

        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderId(
        Guid orderId,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var result = await _mediator.Send(new GetShippingByOrderIdQuery(orderId), ct);
        if (result.IsFailure)
            return NotFound(result.Error);

        if (result.Value!.SellerId != seller.Id.Value)
            return Forbid();

        return Ok(result.Value);
    }

    [HttpPut("{shippingId:guid}/sender")]
    public async Task<IActionResult> SetSender(
        Guid shippingId,
        [FromBody] SetShippingSenderRequest request,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var details = await _mediator.Send(new GetShippingByIdQuery(shippingId), ct);
        if (details.IsFailure)
            return NotFound(details.Error);

        if (details.Value!.SellerId != seller.Id.Value)
            return Forbid();

        var result = await _mediator.Send(new SetShippingSenderCommand(
            shippingId,
            request.Name,
            request.Phone,
            request.CountryCode,
            request.PickupPointCode,
            request.PickupPointName), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPut("{shippingId:guid}/parcels")]
    public async Task<IActionResult> SetParcels(
        Guid shippingId,
        [FromBody] SetShippingParcelsRequest request,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var details = await _mediator.Send(new GetShippingByIdQuery(shippingId), ct);
        if (details.IsFailure)
            return NotFound(details.Error);

        if (details.Value!.SellerId != seller.Id.Value)
            return Forbid();

        var command = new SetShippingParcelsCommand(
            shippingId,
            request.Parcels.Select(x => new SetShippingParcelItem(
                x.RowNumber,
                x.CargoCategory,
                x.Description,
                x.InsuranceCost,
                x.Width,
                x.Length,
                x.Height,
                x.ActualWeight,
                x.VolumetricWeight)).ToArray());

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpPost("{shippingId:guid}/dispatch")]
    public async Task<IActionResult> Dispatch(
        Guid shippingId,
        [FromBody] DispatchShippingRequest request,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var details = await _mediator.Send(new GetShippingByIdQuery(shippingId), ct);
        if (details.IsFailure)
            return NotFound(details.Error);

        if (details.Value!.SellerId != seller.Id.Value)
            return Forbid();

        var result = await _mediator.Send(new DispatchShippingCommand(
            shippingId,
            request.TrackingNumber), ct);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return NoContent();
    }

    [HttpGet("nova-poshta/cities")]
    public async Task<IActionResult> SearchCities(
        [FromQuery] string query,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var result = await _mediator.Send(new SearchNovaPoshtaCitiesQuery(query), ct);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("nova-poshta/warehouses")]
    public async Task<IActionResult> SearchWarehouses(
        [FromQuery] string cityRef,
        [FromQuery] string? query,
        CancellationToken ct = default)
    {
        var seller = await ResolveCurrentSellerAsync(ct);
        if (seller is null)
            return Forbid();

        var result = await _mediator.Send(new SearchNovaPoshtaWarehousesQuery(cityRef, query), ct);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    private async Task<BazaR.Backend.Domain.Sellers.Seller?> ResolveCurrentSellerAsync(CancellationToken ct)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId == Guid.Empty)
            return null;

        return await _sellers.GetByOwnerUserIdAsync(new UserId(_currentUser.UserId), ct);
    }
}