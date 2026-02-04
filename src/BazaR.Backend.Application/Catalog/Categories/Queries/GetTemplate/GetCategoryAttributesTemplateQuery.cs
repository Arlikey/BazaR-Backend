using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetTemplate;

public sealed record GetCategoryAttributesTemplateQuery(CategoryId CategoryId)
    : IRequest<Result<IReadOnlyList<CategoryAttributeTemplateItemDto>>>;
