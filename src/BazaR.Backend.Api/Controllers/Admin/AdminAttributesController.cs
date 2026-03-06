using BazaR.Backend.Api.Contracts.Attributes;
using BazaR.Backend.Api.Contracts.Common;
using BazaR.Backend.Application.Catalog.Attributes.Commands.AddAttributeOptions;
using BazaR.Backend.Application.Catalog.Attributes.Commands.CreateAttributeDefinition;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/attributes")]
[Authorize(Policy = "Admin")]
public sealed class AdminAttributesController : ControllerBase
{
    private readonly IMediator _mediator;
    public AdminAttributesController(IMediator mediator) => _mediator = mediator;

   
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttributeDefinitionRequest request, CancellationToken ct)
    {
        var cmd = new CreateAttributeDefinitionCommand(
            request.Name,
            request.Code,
            request.ValueType,
            request.Unit,
            request.IsSystem
        );
        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Created("", new IdResponse(result.Value));
    }

    
    [HttpPost("{attributeId:guid}/options")]
    public async Task<IActionResult> AddOptions(Guid attributeId, [FromBody] AddOptionsRequest request, CancellationToken ct)
    {
        List<AddAttributeOptionItem> options;

        if (request.Options is { Count: > 0 })
        {
            options = request.Options
                .Select(o => new AddAttributeOptionItem(o.Value, o.SortOrder))
                .ToList();
        }
        else if (!string.IsNullOrWhiteSpace(request.Value))
        {
            options = new List<AddAttributeOptionItem>
            {
                new AddAttributeOptionItem(request.Value, request.SortOrder)
            };
        }
        else
        {
            return Problem(
                title: "Validation",
                detail: "Provide either 'value' or a non-empty 'options' array.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var cmd = new AddAttributeOptionsCommand(attributeId, options);
        var result = await _mediator.Send(cmd, ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(new { optionIds = result.Value });
    }

    
    private static int MapStatus(string code) => code switch
    {
        "Attribute.NotFound" => StatusCodes.Status404NotFound,
        "Attribute.CodeAlreadyExists" => StatusCodes.Status409Conflict,
        "Attribute.OptionAlreadyExists" => StatusCodes.Status409Conflict,
        "Attribute.OptionsNotAllowedForType" => StatusCodes.Status400BadRequest,
        "Attribute.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
        "CategoryAttribute.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
        "Attribute.CannotModifySystemAttribute" => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status400BadRequest
    };

    private IActionResult ProblemFromError(Error error)
        => Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
}