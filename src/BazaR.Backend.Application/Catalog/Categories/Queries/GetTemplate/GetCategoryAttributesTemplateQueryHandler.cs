using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetTemplate;

public sealed class GetCategoryAttributesTemplateQueryHandler
    : IRequestHandler<GetCategoryAttributesTemplateQuery, Result<IReadOnlyList<CategoryAttributeTemplateItemDto>>>
{
    private readonly ICategoryReadRepository _read;

    public GetCategoryAttributesTemplateQueryHandler(ICategoryReadRepository read) => _read = read;

    public async Task<Result<IReadOnlyList<CategoryAttributeTemplateItemDto>>> Handle(GetCategoryAttributesTemplateQuery request, CancellationToken ct)
        => Result<IReadOnlyList<CategoryAttributeTemplateItemDto>>.Success(
            await _read.GetAttributesTemplateAsync(request.CategoryId, ct));
}
