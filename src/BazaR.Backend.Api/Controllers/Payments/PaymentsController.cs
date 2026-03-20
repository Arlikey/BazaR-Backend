using BazaR.Backend.Api.Contracts.Payments;
using BazaR.Backend.Application.Payments.Commands.CancelPayment;
using BazaR.Backend.Application.Payments.Commands.CreatePayment;
using BazaR.Backend.Application.Payments.Commands.RefundPayment;
using BazaR.Backend.Application.Payments.Commands.StartLiqPayCheckout;
using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Application.Payments.Queries.GetPaymentById;
using BazaR.Backend.Application.Payments.Queries.GetPaymentsByOrder;
using BazaR.Backend.Application.Payments.Queries.GetPaymentsBySeller;
using BazaR.Backend.Application.Payments.Queries.GetPaymentsByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public sealed class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePaymentRequest request,
        CancellationToken ct)
    {
        var command = new CreatePaymentCommand(
            request.OrderId,
            request.SellerId,
            request.UserId,
            request.Provider,
            request.Method,
            request.Amount,
            request.Currency,
            request.MerchantOrderReference);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(new CreatePaymentResponse(result.Value));
    }

    [HttpPost("{paymentId:guid}/liqpay/checkout")]
    public async Task<IActionResult> StartLiqPayCheckout(
        Guid paymentId,
        [FromBody] StartLiqPayCheckoutRequest request,
        CancellationToken ct)
    {
        var command = new StartLiqPayCheckoutCommand(
            paymentId,
            request.PayType,
            request.Description);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(new StartLiqPayCheckoutResponse(
            result.Value.PaymentId,
            result.Value.ActionUrl,
            result.Value.Data,
            result.Value.Signature));
    }

    [HttpPost("{paymentId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid paymentId,
        [FromBody] CancelPaymentRequest request,
        CancellationToken ct)
    {
        var command = new CancelPaymentCommand(
            paymentId,
            request.ExternalStatus);

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

    [HttpPost("{paymentId:guid}/refund")]
    public async Task<IActionResult> Refund(
        Guid paymentId,
        [FromBody] RefundPaymentRequest request,
        CancellationToken ct)
    {
        var command = new RefundPaymentCommand(
            paymentId,
            request.Amount,
            request.Currency);

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

    [HttpGet("{paymentId:guid}")]
    public async Task<IActionResult> GetById(
        Guid paymentId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPaymentByIdQuery(paymentId), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(MapToResponse(result.Value));
    }

    [HttpGet("by-order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(
        Guid orderId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPaymentsByOrderQuery(orderId), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value.Select(MapToResponse).ToList());
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetByUser(
        Guid userId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPaymentsByUserQuery(userId), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value.Select(MapToResponse).ToList());
    }

    [HttpGet("by-seller/{sellerId:guid}")]
    public async Task<IActionResult> GetBySeller(
        Guid sellerId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPaymentsBySellerQuery(sellerId), ct);

        if (result.IsFailure)
        {
            return Problem(
                title: result.Error.Code,
                detail: result.Error.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value.Select(MapToResponse).ToList());
    }


    [HttpGet("/result")]
    [AllowAnonymous]
    public IActionResult PaymentResult()
    {
        return Content("Payment finished");
    }
    private static PaymentResponse MapToResponse(PaymentDto dto)
    {
        return new PaymentResponse(
            dto.Id,
            dto.OrderId,
            dto.SellerId,
            dto.UserId,
            dto.Provider.ToString(),
            dto.Method.ToString(),
            dto.Status.ToString(),
            dto.Amount,
            dto.Currency,
            dto.RefundedAmount,
            dto.MerchantOrderReference,
            dto.ExternalPaymentId,
            dto.ExternalOrderReference,
            dto.ExternalSessionId,
            dto.ExternalTransactionId,
            dto.ExternalStatus,
            dto.CheckoutActionUrl,
            dto.CheckoutData,
            dto.CheckoutSignature,
            dto.RequestedLiqPayPayType?.ToString(),
            dto.ActualLiqPayPayType?.ToString(),
            dto.CallbackData,
            dto.CallbackSignature,
            dto.CardMask,
            dto.CardBank,
            dto.CardType,
            dto.ProviderAmount,
            dto.ProviderCurrency,
            dto.FailureCode,
            dto.FailureMessage,
            dto.CreatedAtUtc,
            dto.UpdatedAtUtc,
            dto.CheckoutStartedAtUtc,
            dto.CallbackReceivedAtUtc,
            dto.AuthorizedAtUtc,
            dto.PaidAtUtc,
            dto.FailedAtUtc,
            dto.CancelledAtUtc,
            dto.RefundedAtUtc,
            dto.LastProviderSyncAtUtc);
    }
}