using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.Search;

public sealed record SearchCategoriesQuery(string Term, int Limit = 20)
    : IRequest<Result<IReadOnlyList<CategoryListItemDto>>>;
