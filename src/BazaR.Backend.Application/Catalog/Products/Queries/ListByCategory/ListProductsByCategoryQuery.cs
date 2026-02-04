using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.ListByCategory;

public sealed record ListProductsByCategoryQuery(CategoryId CategoryId)
    : IRequest<Result<IReadOnlyList<ProductListItemDto>>>;
