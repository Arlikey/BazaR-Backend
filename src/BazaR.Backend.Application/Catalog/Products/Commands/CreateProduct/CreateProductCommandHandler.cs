using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Catalog;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;
using System.Linq;

namespace BazaR.Backend.Application.Catalog.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<ProductId>>
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;
    private readonly IAttributeDefinitionRepository _attributes;
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;
    private readonly IUnitOfWork _uow;

    public CreateProductCommandHandler(
        IProductRepository products,
        ICategoryRepository categories,
        IAttributeDefinitionRepository attributes,
        ISellerRepository sellers,
        ICurrentUser current,
        IUnitOfWork uow)
    {
        _products = products;
        _categories = categories;
        _attributes = attributes;
        _sellers = sellers;
        _current = current;
        _uow = uow;
    }

    public async Task<Result<ProductId>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<ProductId>.Failure(new Error("Auth.Required", "Authentication required."));

        //1️ Получаем продавца текущего пользователя
        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<ProductId>.Failure(new Error("Seller.NotFound", "Seller not found."));

        if (seller.Status != SellerStatus.Active)
            return Result<ProductId>.Failure(new Error("Seller.NotActive", "Seller must be active to create products."));

        var ownerSellerId = seller.Id;

        // 2️ Загружаем категорию с шаблоном атрибутов
        var category = await _categories.GetByIdWithAttributesAsync(request.CategoryId, ct);
        if (category is null)
            return Result<ProductId>.Failure(CategoryErrors.NotFound);

        // 3️ Проверка атрибутов
        var inputs = request.Attributes ?? Array.Empty<ProductAttributeInput>();

        var incomingGuids = inputs.Select(x => x.AttributeId).ToList();
        if (incomingGuids.Count != incomingGuids.Distinct().Count())
            return Result<ProductId>.Failure(ProductErrors.DuplicateAttributeInRequest);

        var incomingSet = incomingGuids.ToHashSet();

        var requiredGuids = category.Attributes
            .Where(a => a.IsRequired)
            .Select(a => a.AttributeId.Value)
            .ToHashSet();

        var missing = requiredGuids.Where(req => !incomingSet.Contains(req)).ToList();
        if (missing.Count > 0)
        {
            return Result<ProductId>.Failure(new Error(
                "Product.RequiredAttributesMissing",
                $"Missing required attributes: {string.Join(", ", missing)}"
            ));
        }

        // 4️ Создание продукта
        BrandId? brandId = request.BrandId.HasValue
            ? new BrandId(request.BrandId.Value)
            : null;

        var productRes = Product.CreateBySeller(
            ownerSellerId: ownerSellerId,
            name: request.Name,
            categoryId: request.CategoryId,
            brandId: brandId,
            description: request.Description,
            slug: request.Slug,
            vendorCode: request.VendorCode,
            barcode: request.Barcode
        );

        if (productRes.IsFailure)
            return Result<ProductId>.Failure(productRes.Error);

        var product = productRes.Value!;

        // 5️ Проверка уникальности slug внутри продавца
        if (product.Slug is not null)
        {
            var exists = await _products.SlugExistsForSellerAsync(
                ownerSellerId,
                product.Slug,
                null,
                ct);

            if (exists)
                return Result<ProductId>.Failure(ProductErrors.SlugAlreadyExists);
        }

        // 6️ Проверка уникальности vendorCode внутри продавца
        if (product.VendorCode is not null)
        {
            var exists = await _products.VendorCodeExistsForSellerAsync(
                ownerSellerId,
                product.VendorCode,
                null,
                ct);

            if (exists)
                return Result<ProductId>.Failure(ProductErrors.VendorCodeAlreadyExists);
        }

        // 7️ Загрузка определений атрибутов
        var attrIds = incomingSet.Select(x => new AttributeId(x)).ToList();
        var defs = await _attributes.GetByIdsAsync(attrIds, ct);
        var defMap = defs.ToDictionary(d => d.Id, d => d);

        foreach (var id in attrIds)
        {
            if (!defMap.ContainsKey(id))
                return Result<ProductId>.Failure(AttributeErrors.NotFound);
        }

        // 8️ Установка атрибутов
        foreach (var input in inputs)
        {
            var def = defMap[new AttributeId(input.AttributeId)];

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

        // 9️ Сохраняем
        _products.Add(product);
        await _uow.SaveChangesAsync(ct);

        return Result<ProductId>.Success(product.Id);
    }
}


