using BazaR.Backend.Api.Contracts.Admin.Brands;
using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Catalog.Brands.Commands.ActivateBrand;
using BazaR.Backend.Application.Catalog.Brands.Commands.ArchiveBrand;
using BazaR.Backend.Application.Catalog.Brands.Commands.CreateBrand;
using BazaR.Backend.Application.Catalog.Brands.Commands.UpdateBrand;
using BazaR.Backend.Application.Catalog.Brands.Queries.GetBrandById;
using BazaR.Backend.Application.Catalog.Brands.Queries.ListBrands;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/catalog/brands")]
[Authorize(Roles = "Admin")]
public sealed class BrandsAdminController : ControllerBase
{
    private readonly ISender _sender;

    public BrandsAdminController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    [FromForm] CreateBrandRequest request,
    CancellationToken ct)
    {
        UploadFile? upload = null;

        if (request.Logo is not null)
        {
            upload = new UploadFile(
                request.Logo.OpenReadStream(),
                request.Logo.FileName,
                request.Logo.ContentType,
                request.Logo.Length);
        }

        var result = await _sender.Send(new CreateBrandCommand(
            request.Name,
            request.Slug,
            upload,
            request.Description), ct);

        if (result.IsFailure)
            return BadRequest(ToProblemDetails(result.Error));

        return Ok(new { id = result.Value });
    }

    [HttpPost("{brandId:guid}/archive")]
    public async Task<IActionResult> Archive(
        [FromRoute] Guid brandId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new ArchiveBrandCommand(brandId), ct);

        if (result.IsFailure)
            return BadRequest(ToProblemDetails(result.Error));

        return NoContent();
    }

    [HttpPost("{brandId:guid}/activate")]
    public async Task<IActionResult> Activate(
        [FromRoute] Guid brandId,
        CancellationToken ct)
    {
        var result = await _sender.Send(new ActivateBrandCommand(brandId), ct);

        if (result.IsFailure)
            return BadRequest(ToProblemDetails(result.Error));

        return NoContent();
    }

    [HttpGet("{brandId:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid brandId,
        CancellationToken ct)
    {
        var brand = await _sender.Send(new GetBrandByIdQuery(brandId), ct);
        if (brand is null)
            return NotFound();

        return Ok(brand);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new ListBrandsQuery(
            search,
            status,
            page,
            pageSize), ct);

        return Ok(result);
    }

    private static ProblemDetails ToProblemDetails(Error error)
    {
        return new ProblemDetails
        {
            Title = error.Code,
            Status = StatusCodes.Status400BadRequest
        };
    }
}