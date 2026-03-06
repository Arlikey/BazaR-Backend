using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Categories;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.ListByCategory;

public sealed record ListProductsByCategoryQuery(
    CategoryId CategoryId,
    ProductStatus? Status
) : IRequest<Result<IReadOnlyList<ProductCardWithOfferDto>>>;