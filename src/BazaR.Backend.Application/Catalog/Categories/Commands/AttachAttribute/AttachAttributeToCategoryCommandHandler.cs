using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.AttachAttribute;

public sealed class AttachAttributeToCategoryCommandHandler
    : IRequestHandler<AttachAttributeToCategoryCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IAttributeDefinitionRepository _attributes;
    private readonly IUnitOfWork _uow;

    public AttachAttributeToCategoryCommandHandler(
        ICategoryRepository categories,
        IAttributeDefinitionRepository attributes,
        IUnitOfWork uow)
    {
        _categories = categories;
        _attributes = attributes;
        _uow = uow;
    }

    public async Task<Result> Handle(AttachAttributeToCategoryCommand request, CancellationToken ct)
    {
        // Приводим входные Guid к доменным идентификаторам
        var categoryId = new CategoryId(request.CategoryId);
        var attributeId = new AttributeId(request.AttributeId);

        // Загружаем категорию вместе с уже привязанными атрибутами
        // (нужно для проверки дублей/инвариантов внутри AddAttribute)
        var category = await _categories.GetByIdWithAttributesAsync(categoryId, ct);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        // Проверяем, что атрибут вообще существует
        if (!await _attributes.ExistsAsync(attributeId, ct))
            return Result.Failure(AttributeErrors.NotFound);

        // Доменная операция: привязка атрибута к категории с настройками
        var res = category.AddAttribute(
            attributeId,
            request.IsRequired,
            request.IsFilterable,
            request.SortOrder,
            request.SectionName,
            request.SectionOrder);

        // Если домен запретил операцию — возвращаем ошибку
        if (res.IsFailure)
            return res;

        // Сохраняем изменения
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
