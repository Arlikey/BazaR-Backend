using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.List;

public sealed record ListProductsQuery()
    : IRequest<Result<IReadOnlyList<ProductListItemDto>>>;
