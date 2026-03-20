using BazaR.Backend.Api.Contracts.Sellers;
using BazaR.Backend.Application.PaymentProfiles.Commands.ActivatePaymentProfile;
using BazaR.Backend.Application.PaymentProfiles.Commands.AddPaymentMethod;
using BazaR.Backend.Application.PaymentProfiles.Commands.ArchivePaymentProfile;
using BazaR.Backend.Application.PaymentProfiles.Commands.CreatePaymentProfile;
using BazaR.Backend.Application.PaymentProfiles.Commands.DisablePaymentMethod;
using BazaR.Backend.Application.PaymentProfiles.Commands.EnablePaymentMethod;
using BazaR.Backend.Application.PaymentProfiles.Commands.SetBankAccount;
using BazaR.Backend.Application.PaymentProfiles.Commands.SetLiqPaySettings;
using BazaR.Backend.Application.PaymentProfiles.Commands.SuspendPaymentProfile;
using BazaR.Backend.Application.PaymentProfiles.Commands.UpdatePaymentMethod;
using BazaR.Backend.Application.PaymentProfiles.Queries.GetMyPaymentProfile;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.PaymentProfiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Seller;

[ApiController]
[Route("api/seller/payment-profile")]
[Authorize(Roles = "Seller")]
public sealed class SellerPaymentProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public SellerPaymentProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var result = await _mediator.Send(new CreatePaymentProfileCommand(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(new CreatePaymentProfileResponse(result.Value));
    }

    [HttpGet]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMyPaymentProfileQuery(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        var response = new PaymentProfileResponse(
            result.Value.Id,
            result.Value.SellerId,
            result.Value.Status.ToString(),
            result.Value.BankRecipientName,
            result.Value.BankIban,
            result.Value.BankName,
            result.Value.BankTaxNumber,
            result.Value.BankSwift,
            result.Value.BankPurposeTemplate,
            result.Value.HasLiqPaySettings,
            result.Value.LiqPayPublicKey,
            result.Value.LiqPayResultUrl,
            result.Value.LiqPayServerCallbackUrl,
            result.Value.LiqPayCheckoutEnabled,
            result.Value.LiqPayPrivatPayEnabled,
            result.Value.LiqPayInstallmentsEnabled,
            result.Value.CreatedAtUtc,
            result.Value.UpdatedAtUtc,
            result.Value.Methods
                .Select(x => new PaymentMethodResponse(
                    x.MethodType.ToString(),
                    x.IsEnabled,
                    x.RequiresOnlineAuthorization,
                    x.RequiresBankAccount,
                    x.RequiresLiqPay,
                    x.MinAmount,
                    x.MaxAmount,
                    x.Title,
                    x.Description))
                .ToList());

        return Ok(response);
    }

    [HttpPut("bank-account")]
    public async Task<IActionResult> SetBankAccount([FromBody] SetBankAccountRequest request, CancellationToken ct)
    {
        var command = new SetBankAccountCommand(
            request.RecipientName,
            request.Iban,
            request.BankName,
            request.TaxNumber,
            request.Swift,
            request.PurposeTemplate);

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPut("liqpay")]
    public async Task<IActionResult> SetLiqPaySettings([FromBody] SetLiqPaySettingsRequest request, CancellationToken ct)
    {
        var command = new SetLiqPaySettingsCommand(
            request.PublicKey,
            request.PrivateKey,
            request.ResultUrl,
            request.ServerCallbackUrl,
            request.CheckoutEnabled,
            request.PrivatPayEnabled,
            request.InstallmentsEnabled);

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate(CancellationToken ct)
    {
        var result = await _mediator.Send(new ActivatePaymentProfileCommand(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("suspend")]
    public async Task<IActionResult> Suspend(CancellationToken ct)
    {
        var result = await _mediator.Send(new SuspendPaymentProfileCommand(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("archive")]
    public async Task<IActionResult> Archive(CancellationToken ct)
    {
        var result = await _mediator.Send(new ArchivePaymentProfileCommand(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("methods")]
    public async Task<IActionResult> AddMethod([FromBody] AddPaymentMethodRequest request, CancellationToken ct)
    {
        var command = new AddPaymentMethodCommand(
            request.MethodType,
            request.RequiresOnlineAuthorization,
            request.RequiresBankAccount,
            request.RequiresLiqPay,
            request.MinAmount,
            request.MaxAmount,
            request.Title,
            request.Description);

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPut("methods/{methodType}")]
    public async Task<IActionResult> UpdateMethod(
        PaymentMethodType methodType,
        [FromBody] UpdatePaymentMethodRequest request,
        CancellationToken ct)
    {
        var command = new UpdatePaymentMethodCommand(
            methodType,
            request.RequiresOnlineAuthorization,
            request.RequiresBankAccount,
            request.RequiresLiqPay,
            request.MinAmount,
            request.MaxAmount,
            request.Title,
            request.Description);

        var result = await _mediator.Send(command, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("methods/{methodType}/enable")]
    public async Task<IActionResult> EnableMethod(PaymentMethodType methodType, CancellationToken ct)
    {
        var result = await _mediator.Send(new EnablePaymentMethodCommand(methodType), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("methods/{methodType}/disable")]
    public async Task<IActionResult> DisableMethod(PaymentMethodType methodType, CancellationToken ct)
    {
        var result = await _mediator.Send(new DisablePaymentMethodCommand(methodType), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    private static int MapStatus(string code) => code switch
    {
        "Auth.Required" => StatusCodes.Status401Unauthorized,
        "Auth.Forbidden" => StatusCodes.Status403Forbidden,
        "Seller.NotFound" => StatusCodes.Status404NotFound,
        "PaymentProfile.NotFound" => StatusCodes.Status404NotFound,
        "PaymentProfile.AlreadyExists" => StatusCodes.Status409Conflict,
        "PaymentProfile.InvalidStatus" => StatusCodes.Status409Conflict,
        "BankAccount.Invalid" => StatusCodes.Status400BadRequest,
        "LiqPaySettings.Invalid" => StatusCodes.Status400BadRequest,
        "PaymentMethod.NotFound" => StatusCodes.Status404NotFound,
        "PaymentMethod.AlreadyExists" => StatusCodes.Status409Conflict,
        "PaymentMethod.InvalidConfiguration" => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}