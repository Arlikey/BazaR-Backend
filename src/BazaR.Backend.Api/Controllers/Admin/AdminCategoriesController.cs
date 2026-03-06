using BazaR.Backend.Api.Contracts.Categories;
using BazaR.Backend.Api.Contracts.Common;
using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Catalog.Categories.Commands.CreateCategory;
using BazaR.Backend.Application.Catalog.Categories.Commands.SetImage;
using BazaR.Backend.Application.Catalog.Categories.Queries.GetById; // для CreatedAtAction
using BazaR.Backend.Application.Categories.Commands.RemoveImage;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/catalog/categories")]
[Authorize(Roles = "Admin")]
public sealed class CategoriesAdminController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesAdminController(IMediator mediator) => _mediator = mediator;

    

    // Создание новой категории
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var cmd = new CreateCategoryCommand(request.Name, request.ParentCategoryId, request.SortOrder);
        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return CreatedAtAction(
            nameof(GetById),
            controllerName: "PublicCategories",
            routeValues: new { id = result.Value.Value },
            value: new IdResponse(result.Value.Value));
    }

    
    [HttpPost("{id:guid}/image")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("Only image files are allowed.");

        await using var stream = file.OpenReadStream();
        var upload = new UploadFile(
            Content: stream,
            FileName: file.FileName,
            ContentType: file.ContentType ?? "application/octet-stream",
            SizeBytes: file.Length
        );

        var cmd = new SetCategoryImageCommand(new CategoryId(id), upload);
        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    
    [HttpDelete("{id:guid}/image")]
    public async Task<IActionResult> RemoveImage(Guid id, CancellationToken ct)
    {
        var cmd = new RemoveCategoryImageCommand(new CategoryId(id));
        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);
        return NoContent();
    }

    // Вспомогательный метод для ссылки на публичный контроллер (нужен для CreatedAtAction)
    [NonAction]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        // Этот метод не должен вызываться напрямую, он только для CreatedAtAction.
        var categoryId = new CategoryId(id);
        var result = await _mediator.Send(new GetCategoryByIdQuery(categoryId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

  
    private IActionResult ProblemFromError(Error error)
    {
        var status = error.Code switch
        {
            "Category.NotFound" => StatusCodes.Status404NotFound,
            "Category.ParentCategoryNotFound" => StatusCodes.Status404NotFound,
            "Category.NameRequired" => StatusCodes.Status400BadRequest,
            "Category.NameTooLong" => StatusCodes.Status400BadRequest,
            "Category.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
            "Category.CannotSetSelfAsParent" => StatusCodes.Status400BadRequest,
            "Category.NameAlreadyExists" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };
        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}