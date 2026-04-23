using BazaR.Backend.Application.PaymentProfiles.Queries.GetMyPaymentProfile;
using BazaR.Backend.Application.ShippingProfiles.Queries.GetMyShippingProfile;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/public/")]
[Authorize(Roles = "Customer")]
public sealed class CustomerProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("payment-profile")]
    public async Task<IActionResult> GetMyPaymentProfile(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyPaymentProfileQuery(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        var response = new
        {
            Methods = result.Value.Methods
                .Where(x => x.IsEnabled)
                .Select(x => new
                {
                    Type = x.MethodType.ToString(),
                    MinAmount = x.MinAmount,
                    MaxAmount = x.MaxAmount,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToList()
        };

        return Ok(response);
    }

    [HttpGet("shipping-profile")]
    public async Task<IActionResult> GetMyShippingProfile(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyShippingProfileQuery(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        var response = new
        {
            Methods = result.Value.Methods
                .Where(x => x.IsEnabled)
                .Select(x => new
                {
                    Type = x.MethodType.ToString(),
                    BaseFee = x.BaseFee,
                    Currency = x.Currency,
                    FreeFrom = x.FreeShippingFromAmount,
                    CashOnDelivery = x.AllowCashOnDelivery,
                    EstimatedDaysMin = x.EstimatedDaysMin,
                    EstimatedDaysMax = x.EstimatedDaysMax,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToList()
        };

        return Ok(response);
    }

    private static int MapStatus(string code) => code switch
    {
        "Auth.Required" => StatusCodes.Status401Unauthorized,
        "Auth.Forbidden" => StatusCodes.Status403Forbidden,
        "Seller.NotFound" => StatusCodes.Status404NotFound,
        "PaymentProfile.NotFound" => StatusCodes.Status404NotFound,
        "ShippingProfile.NotFound" => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}