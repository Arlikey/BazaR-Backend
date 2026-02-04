using BazaR.Backend.Api.Contracts.Admin.Attributes;
using BazaR.Backend.Api.Contracts.Admin.Catalog.Attributes;
using BazaR.Backend.Api.Contracts.Common;
using BazaR.Backend.Application.Catalog.Attributes.Commands.AddAttributeOptions;
using BazaR.Backend.Application.Catalog.Attributes.Commands.CreateAttributeDefinition;
using BazaR.Backend.Application.Catalog.Attributes.Queries.GetById;
using BazaR.Backend.Application.Catalog.Attributes.Queries.GetOptions;
using BazaR.Backend.Application.Catalog.Attributes.Queries.List;
using BazaR.Backend.Application.Catalog.Attributes.Queries.Search;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin;

/// <summary>
/// Контроллер для управления атрибутами товаров (админка)
/// </summary>
[ApiController]
[Route("api/admin/attributes")]
public sealed class AttributesAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttributesAdminController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Получить список всех атрибутов
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListAttributeDefinitionsQuery(), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Получить атрибут по ID
    /// </summary>
    [HttpGet("{attributeId:guid}")]
    public async Task<IActionResult> GetById(Guid attributeId, CancellationToken ct)
    {
        // Создаем доменный идентификатор AttributeId из Guid
        var result = await _mediator.Send(new GetAttributeDefinitionByIdQuery(new(attributeId)), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Поиск атрибутов по названию
    /// </summary>
    /// <param name="term">Строка поиска</param>
    /// <param name="limit">Максимальное количество результатов (по умолчанию 20)</param>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int limit = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new SearchAttributeDefinitionsQuery(term, limit), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Получить список опций для атрибута (только для Select/MultiSelect типов)
    /// </summary>
    [HttpGet("{attributeId:guid}/options")]
    public async Task<IActionResult> GetOptions(Guid attributeId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAttributeOptionsQuery(new(attributeId)), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Создать новый атрибут
    /// </summary>
    /// <remarks>
    /// Поддерживаемые типы: Text, Number, Boolean, Select, MultiSelect
    /// </remarks>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttributeDefinitionRequest request, CancellationToken ct)
    {
        // Преобразуем DTO запроса в команду приложения
        var cmd = new CreateAttributeDefinitionCommand(request.Name, request.Code, request.ValueType, request.Unit, request.IsSystem);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        // Возвращаем 201 Created с ID созданного атрибута
        return Created("", new IdResponse(result.Value));
    }

    /// <summary>
    /// Добавить опции к атрибуту
    /// </summary>
    /// <remarks>
    /// Поддерживает два формата запроса:
    /// 1. Одна опция: { "value": "Красный", "sortOrder": 1 }
    /// 2. Несколько опций: { "options": [{ "value": "Красный", "sortOrder": 1 }, ...] }
    /// Только для атрибутов типа Select/MultiSelect
    /// </remarks>
    [HttpPost("{attributeId:guid}/options")]
    public async Task<IActionResult> AddOptions(Guid attributeId, [FromBody] AddOptionsRequest request, CancellationToken ct)
    {
        // Собираем список опций из request
        List<AddAttributeOptionItem> options;

        if (request.Options is { Count: > 0 })
        {
            // Формат с массивом опций
            options = request.Options
                .Select(o => new AddAttributeOptionItem(o.Value, o.SortOrder))
                .ToList();
        }
        else if (!string.IsNullOrWhiteSpace(request.Value))
        {
            // Формат с одной опцией
            options = new List<AddAttributeOptionItem>
            {
                new AddAttributeOptionItem(request.Value, request.SortOrder)
            };
        }
        else
        {
            // Некорректный запрос - нет ни одной опции
            return Problem(
                title: "Validation",
                detail: "Provide either 'value' or a non-empty 'options' array.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        // Отправляем команду добавления опций
        var cmd = new AddAttributeOptionsCommand(attributeId, options);
        var result = await _mediator.Send(cmd, ct);

        if (result.IsFailure)
            return Problem(title: result.Error.Code, detail: result.Error.Message, statusCode: MapStatus(result.Error.Code));

        // Возвращаем массив ID созданных опций
        return Ok(new { optionIds = result.Value });
    }

    /// <summary>
    /// Маппинг кодов ошибок на HTTP статусы
    /// </summary>
    private static int MapStatus(string code) => code switch
    {
        // 404 - Не найдено
        "Attribute.NotFound" => StatusCodes.Status404NotFound,

        // 409 - Конфликт (дублирование)
        "Attribute.CodeAlreadyExists" => StatusCodes.Status409Conflict,
        "Attribute.OptionAlreadyExists" => StatusCodes.Status409Conflict,

        // 400 - Некорректный запрос
        "Attribute.OptionsNotAllowedForType" => StatusCodes.Status400BadRequest,
        "Attribute.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,
        "CategoryAttribute.SortOrderCannotBeNegative" => StatusCodes.Status400BadRequest,

        // 403 - Запрещено
        "Attribute.CannotModifySystemAttribute" => StatusCodes.Status403Forbidden,

        // По умолчанию - 400 Bad Request
        _ => StatusCodes.Status400BadRequest
    };

    /// <summary>
    /// Утилитарный метод для создания Problem из ошибки домена
    /// </summary>
    private IActionResult ProblemFromError(Error error)
    {
        return Problem(title: error.Code, detail: error.Message, statusCode: MapStatus(error.Code));
    }
}