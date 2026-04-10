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

    public CreateCategoryCommandHandler(
        ICategoryRepository categories,
        IUnitOfWork uow)
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

        // 1) Проверка существования родителя
        if (parentId is not null)
        {
            var parentExists = await _categories.ExistsAsync(parentId.Value, ct);
            if (!parentExists)
                return Result<CategoryId>.Failure(CategoryErrors.ParentCategoryNotFound);
        }

        // 2) Уникальность имени среди "соседей"
        var trimmedName = request.Name.Trim();

        var nameExists = await _categories.NameExistsAsync(
            trimmedName,
            parentId,
            excludeCategoryId: null,
            ct);

        if (nameExists)
            return Result<CategoryId>.Failure(CategoryErrors.NameAlreadyExists);

        // 3) Slug
        CategorySlug? slug = null;

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var slugResult = CategorySlug.Create(request.Slug);
            if (slugResult.IsFailure)
                return Result<CategoryId>.Failure(slugResult.Error);

            slug = slugResult.Value;

            var slugExists = await _categories.SlugExistsAsync(
                slug,
                excludeCategoryId: null,
                ct);

            if (slugExists)
                return Result<CategoryId>.Failure(CategoryErrors.SlugAlreadyExists);
        }

        // 4) Создание доменного агрегата
        var categoryResult = Category.Create(
            name: trimmedName,
            parentCategoryId: parentId,
            sortOrder: request.SortOrder,
            slug: slug);

        if (categoryResult.IsFailure)
            return Result<CategoryId>.Failure(categoryResult.Error);

        var category = categoryResult.Value;

        // 5) Сохранение
        await _categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<CategoryId>.Success(category.Id);
    }
}