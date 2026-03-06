using BazaR.Backend.Api.Contracts.Admin;
using BazaR.Backend.Application.Sellers.Commands.ApproveSeller;
using BazaR.Backend.Application.Sellers.Commands.CloseSeller;
using BazaR.Backend.Application.Sellers.Commands.ReactivateSeller;
using BazaR.Backend.Application.Sellers.Commands.RejectSeller;
using BazaR.Backend.Application.Sellers.Commands.SuspendSeller;
using BazaR.Backend.Application.Sellers.Queries.List;
using BazaR.Backend.Domain.Catalog.Products;


/*using BazaR.Backend.Application.Sellers.Queries.GetSellerById;
using BazaR.Backend.Application.Sellers.Queries.ListSellers;*/
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/sellers")]
[Authorize(Policy = "Admin")]
public sealed class AdminSellersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminSellersController(IMediator mediator) => _mediator = mediator;

    
    [HttpGet]
    public async Task<IActionResult> ListSellers(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new ListSellersQuery(status, page, pageSize), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("products")]
    public async Task<IActionResult> ListProducts(
        [FromQuery] Guid? sellerId,
        [FromQuery] ProductStatus? status,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        page = page <= 0 ? 1 : page;
        pageSize = pageSize <= 0 ? 20 : Math.Min(pageSize, 100);

        var result = await _mediator.Send(
            new ListAdminProductsQuery(sellerId, status, search, page, pageSize),
            ct);

        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error.Message);
    }

    
  /*  [HttpGet("{sellerId:guid}")]
    public async Task<IActionResult> GetById(Guid sellerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetSellerByIdQuery(sellerId), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }*/

 
    [HttpPost("{sellerId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid sellerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ApproveSellerCommand(sellerId), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    
    [HttpPost("{sellerId:guid}/reject")]
    public async Task<IActionResult> Reject(Guid sellerId, [FromBody] RejectSellerRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new RejectSellerCommand(sellerId, request.Reason), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

   
    [HttpPost("{sellerId:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid sellerId, [FromBody] SuspendSellerRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new SuspendSellerCommand(sellerId, request.Reason), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    [HttpPost("{sellerId:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid sellerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new ReactivateSellerCommand(sellerId), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

 
    [HttpPost("{sellerId:guid}/close")]
    public async Task<IActionResult> Close(Guid sellerId, CancellationToken ct)
    {
        var result = await _mediator.Send(new CloseSellerCommand(sellerId), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

   
    private static int MapStatus(string code) => code switch
    {
        // Not found
        "Seller.NotFound" => StatusCodes.Status404NotFound,

        // Conflicts
        "Seller.InvalidStatusTransition" => StatusCodes.Status409Conflict,
        "Seller.CannotModifyClosed" => StatusCodes.Status409Conflict,

        // Validation
        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}
