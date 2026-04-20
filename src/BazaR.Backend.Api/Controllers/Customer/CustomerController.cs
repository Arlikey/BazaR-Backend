using BazaR.Backend.Api.Contracts.Customer;
using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Users.Commands.ChangeMyEmail;
using BazaR.Backend.Application.Users.Commands.ChangeMyPhone;
using BazaR.Backend.Application.Users.Commands.RemoveMyAvatar;
using BazaR.Backend.Application.Users.Commands.SetAvatar;

using BazaR.Backend.Application.Users.Commands.UpdateMyProfile;
using BazaR.Backend.Application.Users.Queries.GetMe;
using BazaR.Backend.Application.ViewedProducts.Queries.GetMine;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Customer;

[ApiController]
[Route("api/customer/me")]
[Authorize] 
public sealed class CustomerController : ControllerBase
{
    private const int MaxAvatarBytes = 5 * 1024 * 1024; 
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator) => _mediator = mediator;

    [HttpGet("viewed")]
    public async Task<IActionResult> GetMine(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetMyViewedProductsQuery(page, pageSize),
            ct);

        if (result.IsFailure)
            return Unauthorized(result.Errors);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var res = await _mediator.Send(new GetMeQuery(), ct);
        if (res.IsFailure) return BadRequest(res.Error);
        return Ok(res.Value);
    }

    
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateMyProfileRequest request, CancellationToken ct)
    {
        var res = await _mediator.Send(new UpdateMyProfileCommand(
            FirstName: request.FirstName,
            LastName: request.LastName,
            Phone: request.Phone
        ), ct);

        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }

    
    [HttpPut("email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeMyEmailRequest request, CancellationToken ct)
    {
        var res = await _mediator.Send(new ChangeMyEmailCommand(request.Email), ct);
        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }

    
    [HttpPut("phone")]
    public async Task<IActionResult> ChangePhone([FromBody] ChangeMyPhoneRequest request, CancellationToken ct)
    {
        var res = await _mediator.Send(new ChangeMyPhoneCommand(request.Phone), ct);
        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }

    
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxAvatarBytes)]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        await using var stream = file.OpenReadStream();

       
        var upload = new UploadFile(
            Content: stream,
            FileName: file.FileName,
            ContentType: file.ContentType ?? "application/octet-stream",
            SizeBytes: file.Length
        );

        var res = await _mediator.Send(new SetMyAvatarCommand(upload), ct);
        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }

    
    [HttpDelete("avatar")]
    public async Task<IActionResult> RemoveAvatar(CancellationToken ct)
    {
        var res = await _mediator.Send(new RemoveMyAvatarCommand(), ct);
        if (res.IsFailure) return BadRequest(res.Error);
        return NoContent();
    }


    
}

