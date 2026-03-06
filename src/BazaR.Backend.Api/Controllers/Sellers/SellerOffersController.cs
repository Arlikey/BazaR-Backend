using BazaR.Backend.Api.Contracts.Seller.Offers;
using BazaR.Backend.Application.Offers.Commands.Attributes;
using BazaR.Backend.Application.Offers.Commands.Create;
using BazaR.Backend.Application.Offers.Commands.SetOldPrice;
using BazaR.Backend.Application.Offers.Commands.SetPrice;
using BazaR.Backend.Application.Offers.Commands.Status;
using BazaR.Backend.Application.Offers.Commands.Stock;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Sellers;

[ApiController]
[Route("api/seller/offers")] 
[Authorize(Roles = "Seller")]
public sealed class SellerOffersController : ControllerBase
{
    private readonly IMediator _mediator;
    public SellerOffersController(IMediator mediator) => _mediator = mediator;

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfferRequest request, CancellationToken ct)
    {
        var cmd = new CreateOfferCommand(
            ProductId: request.ProductId,
            PriceAmount: request.PriceAmount,
            PriceCurrency: request.PriceCurrency,
            Stock: request.Stock,
            OldPriceAmount: request.OldPriceAmount,
            OldPriceCurrency: request.OldPriceCurrency,
            SellerSku: request.SellerSku,
            DeliveryDays: request.DeliveryDays,
            MinOrderQuantity: request.MinOrderQuantity,
            Activate: request.Activate
        );
        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return Created("", new { id = result.Value });
    }

    
    [HttpPut("{offerId:guid}/price")]
    public async Task<IActionResult> SetPrice(Guid offerId, [FromBody] SetOfferPriceRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetOfferPriceCommand(offerId, request.Amount, request.Currency), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

   
    [HttpPut("{offerId:guid}/old-price")]
    public async Task<IActionResult> SetOldPrice(Guid offerId, [FromBody] SetOfferOldPriceRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetOfferOldPriceCommand(offerId, request.Amount, request.Currency), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpDelete("{offerId:guid}/old-price")]
    public async Task<IActionResult> ClearOldPrice(Guid offerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ClearOfferOldPriceCommand(offerId), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    
    [HttpPut("{offerId:guid}/stock")]
    public async Task<IActionResult> SetStock(Guid offerId, [FromBody] SetOfferStockRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetOfferStockCommand(offerId, request.Stock), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/stock/increase")]
    public async Task<IActionResult> IncreaseStock(Guid offerId, [FromBody] ChangeOfferStockRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new IncreaseOfferStockCommand(offerId, request.Amount), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/stock/decrease")]
    public async Task<IActionResult> DecreaseStock(Guid offerId, [FromBody] ChangeOfferStockRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new DecreaseOfferStockCommand(offerId, request.Amount), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    
    [HttpPut("{offerId:guid}/sku")]
    public async Task<IActionResult> SetSellerSku(Guid offerId, [FromBody] SetOfferSkuRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetOfferSellerSkuCommand(offerId, request.SellerSku), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPut("{offerId:guid}/delivery")]
    public async Task<IActionResult> SetDelivery(Guid offerId, [FromBody] SetOfferDeliveryDaysRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetOfferDeliveryDaysCommand(offerId, request.DeliveryDays), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPut("{offerId:guid}/min-order")]
    public async Task<IActionResult> SetMinOrder(Guid offerId, [FromBody] SetOfferMinOrderQuantityRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SetOfferMinOrderQuantityCommand(offerId, request.MinOrderQuantity), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    
    [HttpPost("{offerId:guid}/activate")]
    public async Task<IActionResult> Activate(Guid offerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ActivateOfferCommand(offerId), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/pause")]
    public async Task<IActionResult> Pause(Guid offerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new PauseOfferCommand(offerId), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/resume")]
    public async Task<IActionResult> Resume(Guid offerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ResumeOfferCommand(offerId), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    [HttpPost("{offerId:guid}/archive")]
    public async Task<IActionResult> Archive(Guid offerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ArchiveOfferCommand(offerId), ct);
        if (result.IsFailure) return ProblemFromError(result.Error);
        return NoContent();
    }

    
    private static int MapStatus(string code) => code switch
    {
        "Auth.Required" => StatusCodes.Status401Unauthorized,
        "Auth.Forbidden" => StatusCodes.Status403Forbidden,
        "Seller.NotFound" => StatusCodes.Status404NotFound,
        "Seller.NotActive" => StatusCodes.Status409Conflict,
        "Offer.NotFound" => StatusCodes.Status404NotFound,
        "Offer.AlreadyExists" => StatusCodes.Status409Conflict,
        "Offer.InvalidStatusTransition" => StatusCodes.Status409Conflict,
        "Offer.ArchivedCannotBeModified" => StatusCodes.Status409Conflict,
        "Offer.CannotActivateWithoutPrice" => StatusCodes.Status409Conflict,
        "Offer.CannotActivateWithoutStock" => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}