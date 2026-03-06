using BazaR.Backend.Api.Contracts.Attributes;
using BazaR.Backend.Application.Catalog.Attributes.Queries.GetById;
using BazaR.Backend.Application.Catalog.Attributes.Queries.GetOptions;
using BazaR.Backend.Application.Catalog.Attributes.Queries.List;
using BazaR.Backend.Application.Catalog.Attributes.Queries.Search;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/catalog/attributes")]
[AllowAnonymous] 
public sealed class PublicAttributesController : ControllerBase
{
    private readonly IMediator _mediator;
    public PublicAttributesController(IMediator mediator) => _mediator = mediator;

   
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListAttributeDefinitionsQuery(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

   
    [HttpGet("{attributeId:guid}")]
    public async Task<IActionResult> GetById(Guid attributeId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAttributeDefinitionByIdQuery(new(attributeId)), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new SearchAttributeDefinitionsQuery(term, limit), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);
        return Ok(result.Value);
    }

   
    // Получить список опций для атрибута 
  
    [HttpGet("{attributeId:guid}/options")]
    public async Task<IActionResult> GetOptions(Guid attributeId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAttributeOptionsQuery(new(attributeId)), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);
        return Ok(result.Value);
    }
    private IActionResult ProblemFromError(Error error)
    {
        var status = error.Code switch
        {
            "Attribute.NotFound" => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };
        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}