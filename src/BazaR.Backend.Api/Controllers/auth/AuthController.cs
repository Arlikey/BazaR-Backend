using BazaR.Backend.Application.Identity.Commands.Login;
using BazaR.Backend.Application.Identity.Commands.Logout;
using BazaR.Backend.Application.Identity.Commands.Refresh;
using BazaR.Backend.Application.Identity.Commands.Register;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
    {
        var reg = await _mediator.Send(
            new RegisterCommand(
                Email: req.Email,
                Password: req.Password,
                FirstName: req.FirstName,
                LastName: req.LastName,
                Phone: req.Phone
            ),
            ct);

        if (reg.IsFailure) return ProblemFromError(reg.Error);

        // Авто-логин: используем тот же юзкейс Login
        var login = await _mediator.Send(new LoginCommand(req.Email, req.Password), ct);
        if (login.IsFailure) return ProblemFromError(login.Error);
        return Ok(login.Value);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(req.Email, req.Password), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(req.UserId, req.RefreshToken), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(new LogoutCommand(req.UserId, req.RefreshToken), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    private static int MapStatus(string code) => code switch
    {
        "Identity.EmailTaken" => StatusCodes.Status409Conflict,
        "User.EmailTaken" => StatusCodes.Status409Conflict,
        "User.PhoneTaken" => StatusCodes.Status409Conflict,

        "Auth.InvalidCredentials" => StatusCodes.Status401Unauthorized,
        "Auth.InvalidRefresh" => StatusCodes.Status401Unauthorized,
        "Auth.Blocked" => StatusCodes.Status403Forbidden,

        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));

    public sealed record RegisterRequest(
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string? Phone = null
    );

    public sealed record LoginRequest(string Email, string Password);
    public sealed record RefreshRequest(Guid UserId, string RefreshToken);
}
