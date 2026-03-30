using BazaR.Backend.Api.Contracts.Categories;
using BazaR.Backend.Application.Catalog.Categories.Queries.GetById;
using BazaR.Backend.Application.Catalog.Categories.Queries.GetTemplate;
using BazaR.Backend.Application.Catalog.Categories.Queries.List;
using BazaR.Backend.Application.Catalog.Categories.Queries.Search;
using BazaR.Backend.Application.Catalog.Categories.Queries.Tree;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Public;

[ApiController]
[Route("api/catalog/categories")]
[AllowAnonymous]
public sealed class PublicCategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicCategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListCategoriesQuery(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        var response = result.Value
            .Select(x => new CategoryListItemResponse(
                x.Id,
                x.Name,
                x.ParentCategoryId,
                x.SortOrder,
                x.ImageUrl))
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var categoryId = new CategoryId(id);
        var result = await _mediator.Send(new GetCategoryByIdQuery(categoryId), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesTreeQuery(), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new SearchCategoriesQuery(term, limit), ct);
        if (result.IsFailure)
            return ProblemFromError(result.Error);

        var response = result.Value
            .Select(x => new CategoryListItemResponse(
                x.Id,
                x.Name,
                x.ParentCategoryId,
                x.SortOrder,
                x.ImageUrl))
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}/attributes-template")]
    public async Task<IActionResult> GetAttributesTemplate(Guid id, CancellationToken ct)
    {
        var categoryId = new CategoryId(id);
        var result = await _mediator.Send(new GetCategoryAttributesTemplateQuery(categoryId), ct);
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
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}