using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Catalog.Attributes;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Commands.UpdateCategoryAttribute;

public sealed class UpdateCategoryAttributeRulesCommandHandler
    : IRequestHandler<UpdateCategoryAttributeRulesCommand, Result>
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _uow;

    public UpdateCategoryAttributeRulesCommandHandler(
        ICategoryRepository categories,
        IUnitOfWork uow)
    {
        _categories = categories;
        _uow = uow;
    }

    public async Task<Result> Handle(UpdateCategoryAttributeRulesCommand request, CancellationToken ct)
    {
        var categoryId = new CategoryId(request.CategoryId);
        var attributeId = new AttributeId(request.AttributeId);

        var category = await _categories.GetByIdWithAttributesAsync(categoryId, ct);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        // 1) rules
        var res1 = category.UpdateAttributeRules(attributeId, request.IsRequired, request.IsFilterable);
        if (res1.IsFailure) return res1;

        // 2) sort order
        var res2 = category.SetAttributeSortOrder(attributeId, request.SortOrder);
        if (res2.IsFailure) return res2;

        // 3) section
        var res3 = category.SetAttributeSection(attributeId, request.SectionName, request.SectionOrder);
        if (res3.IsFailure) return res3;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
