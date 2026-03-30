using BazaR.Backend.Api.Contracts.Sellers;
using BazaR.Backend.Application.Sellers.Commands.ChangeMySellerSlug;
using BazaR.Backend.Application.Sellers.Commands.CreateSeller;
using BazaR.Backend.Application.Sellers.Commands.RenameMySeller;
using BazaR.Backend.Application.Sellers.Commands.SubmitForApproval;
using BazaR.Backend.Application.Sellers.Commands.UpdateMySellerContacts;
using BazaR.Backend.Application.Sellers.Commands.UpdateMySellerLegal;
//using BazaR.Backend.Application.Sellers.Commands.UpdateMySellerProfile;
using BazaR.Backend.Application.Sellers.Queries.GetMySeller;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Sellers;

[ApiController]
[Route("api/seller/me")]
[Authorize(Roles = "Seller")]
public sealed class SellerMeController : ControllerBase
{
    private readonly IMediator _mediator;
    public SellerMeController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMySellerQuery(), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSellerRequest request, CancellationToken ct)
    {
        var cmd = new CreateSellerCommand(
            Name: request.Name,
            Slug: request.Slug,
            Type: Domain.Sellers.SellerType.Regular,
            CountryCode: request.CountryCode,

            Description: request.Description,
            LogoUrl: request.LogoUrl,

            LegalName: request.LegalName,
            TaxNumber: request.TaxNumber,

            SupportEmail: request.SupportEmail,
            SupportPhone: request.SupportPhone,

            SubmitForApproval: request.SubmitForApproval
        );

        var result = await _mediator.Send(cmd, ct);

        if (result.IsFailure) return ProblemFromError(result.Error);
        return Created("", new { id = result.Value });
    }



    [HttpPut("name")]
    public async Task<IActionResult> Rename([FromBody] RenameSellerRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RenameMySellerCommand(request.Name), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPut("slug")]
    public async Task<IActionResult> ChangeSlug([FromBody] ChangeSellerSlugRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new ChangeMySellerSlugCommand(request.Slug), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }


    // доделать профиль продавца
   /* 
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateSellerProfileRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateMySellerProfileCommand(request.Description, request.LogoUrl), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }*/

    [HttpPut("legal")]
    public async Task<IActionResult> UpdateLegal([FromBody] UpdateSellerLegalRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new UpdateMySellerLegalCommand(request.LegalName, request.TaxNumber, request.CountryCode), ct);

        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPut("contacts")]
    public async Task<IActionResult> UpdateContacts([FromBody] UpdateSellerContactsRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateMySellerContactsCommand(request.Email, request.Phone), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitForApproval(CancellationToken ct)
    {
        var result = await _mediator.Send(new SubmitSellerForApprovalCommand(), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    private static int MapStatus(string code) => code switch
    {
        "Auth.Required" => StatusCodes.Status401Unauthorized,
        "Auth.Forbidden" => StatusCodes.Status403Forbidden,

        "Seller.NotFound" => StatusCodes.Status404NotFound,

        "Seller.AlreadyExists" => StatusCodes.Status409Conflict,
        "Seller.SlugTaken" => StatusCodes.Status409Conflict,
        "Seller.InvalidStatusTransition" => StatusCodes.Status409Conflict,
        "Seller.CannotModifyClosed" => StatusCodes.Status409Conflict,

      
        "Seller.ContactsRequired" => StatusCodes.Status409Conflict,
        "Seller.DescriptionTooLong" => StatusCodes.Status400BadRequest,
        "Seller.EmailInvalid" => StatusCodes.Status400BadRequest,
        "Seller.PhoneInvalid" => StatusCodes.Status400BadRequest,
        "Seller.OwnerRequired" => StatusCodes.Status400BadRequest,

        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}
