using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categories,
        IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var categoryId = new CategoryId(request.CategoryId);

        // 1) Получаем категорию
        var category = await _categories.GetByIdAsync(categoryId, ct);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        // 2) Parent
        CategoryId? parentId = request.ParentCategoryId is null
            ? null
            : new CategoryId(request.ParentCategoryId.Value);

        if (parentId is not null)
        {
            var parentExists = await _categories.ExistsAsync(parentId.Value, ct);
            if (!parentExists)
                return Result.Failure(CategoryErrors.ParentCategoryNotFound);

            var isDescendant = await _categories.IsDescendantAsync(
                ancestorId: categoryId,
                possibleDescendantId: parentId.Value,
                ct);

            if (isDescendant)
                return Result.Failure(CategoryErrors.CannotSetDescendantAsParent);
        }

        // 3) Name uniqueness
        var trimmedName = request.Name.Trim();

        var nameExists = await _categories.NameExistsAsync(
            trimmedName,
            parentId,
            excludeCategoryId: categoryId,
            ct);

        if (nameExists)
            return Result.Failure(CategoryErrors.NameAlreadyExists);

        // 4) Slug
        if (request.Slug is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Slug))
            {
                var clearSlugResult = category.SetSlug(null);
                if (clearSlugResult.IsFailure)
                    return clearSlugResult;
            }
            else
            {
                var slugResult = CategorySlug.Create(request.Slug);
                if (slugResult.IsFailure)
                    return Result.Failure(slugResult.Error);

                var slug = slugResult.Value;

                var slugExists = await _categories.SlugExistsAsync(
                    slug,
                    excludeCategoryId: categoryId,
                    ct);

                if (slugExists)
                    return Result.Failure(CategoryErrors.SlugAlreadyExists);

                var setSlugResult = category.SetSlug(slug);
                if (setSlugResult.IsFailure)
                    return setSlugResult;
            }
        }

        // 5) Rename
        var renameResult = category.Rename(trimmedName);
        if (renameResult.IsFailure)
            return renameResult;

        // 6) Move
        var moveResult = category.Move(parentId, request.SortOrder);
        if (moveResult.IsFailure)
            return moveResult;

        // 7) Save
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}