using BazaR.Backend.Api.Contracts.Admin.Categories;
using BazaR.Backend.Api.Contracts.Common;
using BazaR.Backend.Application.Catalog.Categories.Commands.CreateCategory;
using BazaR.Backend.Application.Catalog.Categories.Queries.List;
using BazaR.Backend.Application.Catalog.Categories.Queries.GetById;
using BazaR.Backend.Application.Catalog.Categories.Queries.Tree;
using BazaR.Backend.Application.Catalog.Categories.Queries.Search;
using BazaR.Backend.Application.Catalog.Categories.Queries.GetTemplate;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/catalog/categories")]
public sealed class CategoriesAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesAdminController(IMediator mediator) => _mediator = mediator;

    // =========================================================
    // COMMANDS
    // =========================================================

    // Создание новой категории
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
    {
        var cmd = new CreateCategoryCommand(request.Name, request.ParentCategoryId, request.SortOrder);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        // Возвращаем 201 Created с ID новой категории
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value.Value },
            new IdResponse(result.Value.Value));
    }

    // =========================================================
    // QUERIES
    // =========================================================

    // Получить список всех категорий (плоский список)
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListCategoriesQuery(), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        // Маппим DTO в API контракт
        var response = result.Value
            .Select(x => new CategoryListItemResponse(x.Id, x.Name, x.ParentCategoryId, x.SortOrder))
            .ToList();

        return Ok(response);
    }

    // Получить категорию по ID (детальная информация)
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var categoryId = new CategoryId(id);
        var result = await _mediator.Send(new GetCategoryByIdQuery(categoryId), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    // Получить дерево категорий (иерархическая структура)
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriesTreeQuery(), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    // Поиск категорий по названию
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new SearchCategoriesQuery(term, limit), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        // Маппим результаты поиска
        var response = result.Value
            .Select(x => new CategoryListItemResponse(x.Id, x.Name, x.ParentCategoryId, x.SortOrder))
            .ToList();

        return Ok(response);
    }

    // Получить шаблон атрибутов для категории (для создания товара)
    [HttpGet("{id:guid}/attributes-template")]
    public async Task<IActionResult> GetAttributesTemplate(Guid id, CancellationToken ct)
    {
        var categoryId = new CategoryId(id);
        var result = await _mediator.Send(new GetCategoryAttributesTemplateQuery(categoryId), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    // =========================================================
    // Обработка ошибок
    // =========================================================

    private IActionResult ProblemFromError(Error error)
    {
        var status = error.Code switch
        {
            // 404 - Не найдено
            "Category.NotFound" => StatusCodes.Status404NotFound,
            "Category.ParentCategoryNotFound" => StatusCodes.Status404NotFound,

            // 400 - Некорректный запрос
            "Category.NameRequired" => StatusCodes.Status400BadRequest,
            "Category.NameTooLong" => StatusCodes.Status400BadRequest,
            "Category.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
            "Category.CannotSetSelfAsParent" => StatusCodes.Status400BadRequest,

            // 409 - Конфликт (дублирование)
            "Category.NameAlreadyExists" => StatusCodes.Status409Conflict,

            // По умолчанию - 400
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}