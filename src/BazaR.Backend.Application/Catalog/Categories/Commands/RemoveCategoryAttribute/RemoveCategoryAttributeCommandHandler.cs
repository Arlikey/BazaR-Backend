using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.RemoveCategoryAttribute;

public sealed class RemoveCategoryAttributeCommandHandler
    : IRequestHandler<RemoveCategoryAttributeCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public RemoveCategoryAttributeCommandHandler(
        ICategoryRepository categories,
        IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<Result> Handle(RemoveCategoryAttributeCommand request, CancellationToken ct)
    {
        // Приводим входные Guid к доменным идентификаторам
        var categoryId = new CategoryId(request.CategoryId);
        var attributeId = new AttributeId(request.AttributeId);

        // Загружаем категорию вместе с её атрибутами
        var category = await _categories.GetByIdWithAttributesAsync(categoryId, ct);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        // Доменная операция: отвязать атрибут от категории
        var res = category.RemoveAttribute(attributeId);
        if (res.IsFailure) return res;

        // Сохраняем изменения
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}
