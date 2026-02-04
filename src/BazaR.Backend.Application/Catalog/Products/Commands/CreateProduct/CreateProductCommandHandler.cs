using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<ProductId>>
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;
    private readonly IAttributeDefinitionRepository _attributes;
    private readonly IUnitOfWork _uow;

    public CreateProductCommandHandler(
        IProductRepository products,
        ICategoryRepository categories,
        IAttributeDefinitionRepository attributes,
        IUnitOfWork uow)
    {
        _products = products;
        _categories = categories;
        _attributes = attributes;
        _uow = uow;
    }

    public async Task<Result<ProductId>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // 1) Загружаем категорию вместе с шаблоном атрибутов
        var categoryId = request.CategoryId;
        var category = await _categories.GetByIdWithAttributesAsync(categoryId, ct);
        if (category is null)
            return Result<ProductId>.Failure(CategoryErrors.NotFound);

        // 2) Формируем BrandId (если передан)
        BrandId? brandId = request.BrandId.HasValue
            ? new BrandId(request.BrandId.Value)
            : null;

        // 3) Создаём агрегат Product (базовые поля)
        var productRes = Product.Create(
            name: request.Name,
            categoryId: categoryId,
            brandId: brandId,
            vendorCode: request.VendorCode,
            slug: request.Slug
        );

        if (productRes.IsFailure)
            return Result<ProductId>.Failure(productRes.Error);

        var product = productRes.Value!;

        // 4) Устанавливаем описание (если передано)
        if (request.Description is not null)
        {
            var descRes = product.ChangeDescription(request.Description);
            if (descRes.IsFailure)
                return Result<ProductId>.Failure(descRes.Error);
        }

        // 5) Проверки уникальности (slug / vendorCode)
        if (product.Slug is not null)
        {
            var exists = await _products.SlugExistsAsync(product.Slug, excludeProductId: null, ct);
            if (exists)
                return Result<ProductId>.Failure(ProductErrors.SlugAlreadyExists);
        }

        if (product.VendorCode is not null)
        {
            var exists = await _products.VendorCodeExistsAsync(product.VendorCode, excludeProductId: null, ct);
            if (exists)
                return Result<ProductId>.Failure(ProductErrors.VendorCodeAlreadyExists);
        }

        // =========================================================
        // 6) Применение атрибутов продукта по шаблону категории
        // =========================================================

        // 6.1 Карта атрибутов категории (разрешённые / обязательные)
        var template = category.Attributes.ToDictionary(a => a.AttributeId, a => a);

        // Входные значения атрибутов (может быть null)
        var inputs = request.Attributes ?? Array.Empty<ProductAttributeInput>();

        // 6.2 Проверка на дубликаты атрибутов в запросе
        var incomingAttrIds = inputs.Select(x => new AttributeId(x.AttributeId)).ToList();
        if (incomingAttrIds.Count != incomingAttrIds.Distinct().Count())
            return Result<ProductId>.Failure(ProductErrors.DuplicateAttributeInRequest);

        // 6.3 Проверка: атрибут разрешён для категории
        foreach (var id in incomingAttrIds)
        {
            if (!template.ContainsKey(id))
                return Result<ProductId>.Failure(ProductErrors.AttributeNotAllowedForCategory);
        }

        // 6.4 Проверка обязательных атрибутов
        var requiredIds = category.Attributes
            .Where(a => a.IsRequired)
            .Select(a => a.AttributeId)
            .ToHashSet();

        foreach (var reqId in requiredIds)
        {
            if (!incomingAttrIds.Contains(reqId))
                return Result<ProductId>.Failure(ProductErrors.RequiredAttributesMissing);
        }

        // 6.5 Загружаем определения атрибутов (включая options)
        var allNeeded = incomingAttrIds.Union(requiredIds).ToList();
        var defs = await _attributes.GetByIdsAsync(allNeeded, ct);
        var defMap = defs.ToDictionary(d => d.Id, d => d);

        // Проверка, что все определения существуют
        foreach (var id in allNeeded)
        {
            if (!defMap.ContainsKey(id))
                return Result<ProductId>.Failure(AttributeErrors.NotFound);
        }

        // 6.6 Устанавливаем значения атрибутов в продукт
        foreach (var input in inputs)
        {
            var attrId = new AttributeId(input.AttributeId);
            var def = defMap[attrId];

            // Валидация значений и options происходит внутри домена
            var res = product.SetAttributeValue(
                def,
                text: input.TextValue,
                number: input.NumberValue,
                boolean: input.BoolValue,
                optionId: input.OptionId,
                optionIds: input.OptionIds
            );

            if (res.IsFailure)
                return Result<ProductId>.Failure(res.Error);
        }

        // 7) Сохраняем продукт
        _products.Add(product);
        await _uow.SaveChangesAsync(ct);

        return Result<ProductId>.Success(product.Id);
    }
}
