
using BazaR.Backend.Api.Contracts.Seller;
using BazaR.Backend.Api.Contracts.Sellers;
using BazaR.Backend.Application.ShippingProfiles.Commands.ActivateShippingProfile;
using BazaR.Backend.Application.ShippingProfiles.Commands.AddShippingMethod;
using BazaR.Backend.Application.ShippingProfiles.Commands.ArchiveShippingProfile;
using BazaR.Backend.Application.ShippingProfiles.Commands.CreateShippingProfile;
using BazaR.Backend.Application.ShippingProfiles.Commands.DisableShippingMethod;
using BazaR.Backend.Application.ShippingProfiles.Commands.EnableShippingMethod;
using BazaR.Backend.Application.ShippingProfiles.Commands.SuspendShippingProfile;
using BazaR.Backend.Application.ShippingProfiles.Commands.UpdateShippingMethod;
using BazaR.Backend.Application.ShippingProfiles.Queries.GetMyShippingProfile;
using BazaR.Backend.Domain.ShippingProfiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Seller;

[ApiController]
[Route("api/seller/shipping-profile")]
[Authorize(Roles = "Seller")]
public sealed class SellerShippingProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public SellerShippingProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateShippingProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateShippingProfileCommand(), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(new CreateShippingProfileResponse(result.Value));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ShippingProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyShippingProfileQuery(), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var response = new ShippingProfileResponse(
            result.Value.Id,
            result.Value.SellerId,
            result.Value.Status.ToString(),
            result.Value.CreatedAtUtc,
            result.Value.UpdatedAtUtc,
            result.Value.Methods
                .Select(x => new ShippingMethodResponse(
                    x.MethodType.ToString(),
                    x.IsEnabled,
                    x.BaseFee,
                    x.Currency,
                    x.FreeShippingFromAmount,
                    x.AllowCashOnDelivery,
                    x.RequiresCity,
                    x.RequiresPickupPoint,
                    x.RequiresStreetAddress,
                    x.EstimatedDaysMin,
                    x.EstimatedDaysMax,
                    x.Title,
                    x.Description))
                .ToList());

        return Ok(response);
    }

    [HttpPost("activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Activate(CancellationToken ct)
    {
        var result = await _mediator.Send(new ActivateShippingProfileCommand(), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("suspend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Suspend(CancellationToken ct)
    {
        var result = await _mediator.Send(new SuspendShippingProfileCommand(), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("archive")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Archive(CancellationToken ct)
    {
        var result = await _mediator.Send(new ArchiveShippingProfileCommand(), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("methods")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddMethod(
        [FromBody] AddShippingMethodRequest request,
        CancellationToken ct)
    {
        var command = new AddShippingMethodCommand(
            request.MethodType,
            request.BaseFee,
            request.Currency,
            request.FreeShippingFromAmount,
            request.AllowCashOnDelivery,
            request.EstimatedDaysMin,
            request.EstimatedDaysMax,
            request.Title,
            request.Description);

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

    [HttpPut("methods/{methodType}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMethod(
        ShippingMethodType methodType,
        [FromBody] UpdateShippingMethodRequest request,
        CancellationToken ct)
    {
        var command = new UpdateShippingMethodCommand(
            methodType,
            request.BaseFee,
            request.Currency,
            request.FreeShippingFromAmount,
            request.AllowCashOnDelivery,
            request.EstimatedDaysMin,
            request.EstimatedDaysMax,
            request.Title,
            request.Description);

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

    [HttpPost("methods/{methodType}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EnableMethod(
        ShippingMethodType methodType,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new EnableShippingMethodCommand(methodType), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    [HttpPost("methods/{methodType}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DisableMethod(
        ShippingMethodType methodType,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new DisableShippingMethodCommand(methodType), ct);

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