using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetBySlug;

public sealed class GetCategoryBySlugQueryHandler
    : IRequestHandler<GetCategoryBySlugQuery, Result<CategoryDetailsDto>>
{
    private readonly ICategoryRepository _categories;

    public GetCategoryBySlugQueryHandler(ICategoryRepository categories)
    {
        _categories = categories;
    }

    public async Task<Result<CategoryDetailsDto>> Handle(GetCategoryBySlugQuery request, CancellationToken ct)
    {
        var category = await _categories.GetBySlugAsync(request.Slug, ct);
        if (category is null)
            return Result<CategoryDetailsDto>.Failure(CategoryErrors.NotFound);

        var dto = new CategoryDetailsDto(
        category.Id.Value,
        category.Name,
        category.Slug?.Value,
        category.ParentCategoryId?.Value,
        category.SortOrder,
        category.Image?.Url,
        new List<CategoryAttributeTemplateItemDto>());

        return Result<CategoryDetailsDto>.Success(dto);
    }
}