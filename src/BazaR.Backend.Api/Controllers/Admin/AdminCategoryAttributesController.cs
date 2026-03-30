using BazaR.Backend.Api.Contracts.Categories.Attributes;
using BazaR.Backend.Application.Catalog.Categories.Commands.AttachAttribute;

using BazaR.Backend.Application.Catalog.Categories.Commands.RemoveCategoryAttribute;
using BazaR.Backend.Application.Catalog.Categories.Commands.UpdateCategoryAttribute;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/catalog/categories/{categoryId:guid}/attributes")]
[Authorize(Policy = "Admin")]
public sealed class AdminCategoryAttributesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminCategoryAttributesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/admin/catalog/categories/{categoryId}/attributes
    [HttpPost]
    public async Task<IActionResult> Attach(Guid categoryId, [FromBody] AttachCategoryAttributeRequest request, CancellationToken ct)
    {
        var cmd = new AttachAttributeToCategoryCommand(
            CategoryId: categoryId,
            AttributeId: request.AttributeId,
            IsRequired: request.IsRequired,
            IsFilterable: request.IsFilterable,
            SortOrder: request.SortOrder,
            SectionName: request.SectionName,
            SectionOrder: request.SectionOrder);

        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    /*// PUT /api/v1/admin/catalog/categories/{categoryId}/attributes/{attributeId}
    [HttpPut("{attributeId:guid}")]
    public async Task<IActionResult> Update(Guid categoryId, Guid attributeId, [FromBody] UpdateCategoryAttributeRequest request, CancellationToken ct)
    {
        var cmd = new UpdateCategoryAttributeCommand(
            CategoryId: categoryId,
            AttributeId: attributeId,
            IsRequired: request.IsRequired,
            IsFilterable: request.IsFilterable,
            SortOrder: request.SortOrder,
            SectionName: request.SectionName,
            SectionOrder: request.SectionOrder);

        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }

    // DELETE /api/v1/admin/catalog/categories/{categoryId}/attributes/{attributeId}
    [HttpDelete("{attributeId:guid}")]
    public async Task<IActionResult> Remove(Guid categoryId, Guid attributeId, CancellationToken ct)
    {
        var cmd = new RemoveCategoryAttributeCommand(categoryId, attributeId);

        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return NoContent();
    }*/

    private IActionResult ProblemFromError(Error error)
    {
        var status = error.Code switch
        {
            "Category.NotFound" => StatusCodes.Status404NotFound,
            "Attribute.NotFound" => StatusCodes.Status404NotFound,

            // дубликат привязки (если вернёшь отдельную ошибку)
            "CategoryAttribute.DuplicateAttributeInCategory" => StatusCodes.Status409Conflict,

            // валидации sortOrder/sectionOrder
            "CategoryAttribute.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
            "CategoryAttribute.SectionOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
            "CategoryAttribute.SectionNameTooLong" => StatusCodes.Status400BadRequest,

            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}
