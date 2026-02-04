using BazaR.Backend.Api.Contracts.Admin.Products;
using BazaR.Backend.Application.Catalog.Products.Commands.CreateProduct;
using BazaR.Backend.Application.Catalog.Products.Queries.List;
using BazaR.Backend.Application.Catalog.Products.Queries.ListByCategory;
using BazaR.Backend.Application.Catalog.Products.Queries.GetById;
using BazaR.Backend.Application.Catalog.Products.Queries.GetAttributesView;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BazaR.Backend.Api.Controllers.Admin.Catalog;

[ApiController]
[Route("api/admin/products")]
public sealed class AdminProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // =========================================================
    // COMMANDS
    // =========================================================

    // Создание нового товара
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken ct)
    {
        // Преобразуем атрибуты из API контракта в DTO приложения
        var attrs = (request.Attributes ?? Array.Empty<CreateProductAttributeRequest>())
            .Select(a => new ProductAttributeInput(
                AttributeId: a.AttributeId,
                TextValue: a.TextValue,
                NumberValue: a.NumberValue,
                BoolValue: a.BoolValue,
                OptionId: a.OptionId,
                OptionIds: a.OptionIds
            ))
            .ToList();

        // Создаем команду с доменными идентификаторами
        var command = new CreateProductCommand(
            Name: request.Name,
            CategoryId: new CategoryId(request.CategoryId),
            BrandId: request.BrandId,
            Description: request.Description,
            VendorCode: request.VendorCode,
            Slug: request.Slug,
            Attributes: attrs
        );

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        // Возвращаем 201 Created с ссылкой на созданный товар
        return CreatedAtAction(
            actionName: nameof(GetById),
            routeValues: new { id = result.Value.Value },
            value: new CreateProductResponse { ProductId = result.Value.Value }
        );
    }

    // =========================================================
    // QUERIES
    // =========================================================

    // Получить список всех товаров
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var result = await _mediator.Send(new ListProductsQuery(), ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    // Получить товары по категории
    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<IActionResult> ListByCategory(Guid categoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ListProductsByCategoryQuery(new CategoryId(categoryId)),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    // Получить товар по ID (детальная информация)
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetProductByIdQuery(new ProductId(id)),
            ct);

        if (result.IsFailure)
            return ProblemFromError(result.Error);

        return Ok(result.Value);
    }

    // Получить атрибуты товара (отдельный запрос для просмотра значений)
    [HttpGet("{id:guid}/attributes")]
    public async Task<IActionResult> GetAttributes(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetProductAttributesViewQuery(new ProductId(id)),
            ct);

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
            "Product.NotFound" => StatusCodes.Status404NotFound,
            "Category.NotFound" => StatusCodes.Status404NotFound,

            // 409 - Конфликт (дублирование)
            "Product.SlugAlreadyExists" => StatusCodes.Status409Conflict,
            "Product.VendorCodeAlreadyExists" => StatusCodes.Status409Conflict,

            // 400 - Некорректный запрос (по умолчанию)
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: status);
    }
}