using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.GetBySlug;

public sealed record GetCategoryBySlugQuery(CategorySlug Slug)
    : IRequest<Result<CategoryDetailsDto>>;