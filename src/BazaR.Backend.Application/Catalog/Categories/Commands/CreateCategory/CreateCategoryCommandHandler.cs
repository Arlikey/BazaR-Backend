using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<CategoryId>>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public CreateCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<Result<CategoryId>> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        // Guid? -> CategoryId?
        CategoryId? parentId = request.ParentCategoryId is null
            ? null
            : new CategoryId(request.ParentCategoryId.Value);

        // 1) Проверка существования родителя (если указан)
        if (parentId is not null)
        {
            var parentExists = await _categories.ExistsAsync(parentId.Value, ct);
            if (!parentExists)
                return Result<CategoryId>.Failure(CategoryErrors.ParentCategoryNotFound);
        }

        // 2) Уникальность имени среди "соседей" (один parent)
        var nameExists = await _categories.NameExistsAsync(
            request.Name.Trim(),
            parentId,
            excludeCategoryId: null,
            ct);

        if (nameExists)
            return Result<CategoryId>.Failure(CategoryErrors.NameAlreadyExists);

        // 3) Создание доменного агрегата
        var categoryResult = Category.Create(
            name: request.Name,
            parentCategoryId: parentId,
            sortOrder: request.SortOrder);

        if (categoryResult.IsFailure)
            return Result<CategoryId>.Failure(categoryResult.Error);

        var category = categoryResult.Value;

        // 4) Сохранение
        await _categories.AddAsync(category, ct); 
        await _uow.SaveChangesAsync(ct);

        return Result<CategoryId>.Success(category.Id);
    }
}
