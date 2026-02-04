using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Categories.Queries.Tree;

public sealed record GetCategoriesTreeQuery()
    : IRequest<Result<IReadOnlyList<CategoryTreeNodeDto>>>;
