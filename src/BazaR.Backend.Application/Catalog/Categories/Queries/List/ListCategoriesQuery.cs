using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.List;

public sealed record ListCategoriesQuery()
    : IRequest<Result<IReadOnlyList<CategoryListItemDto>>>;
