using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.GetById;

public sealed record GetMyProductByIdQuery(
    SellerId SellerId,
    ProductId ProductId
) : IRequest<Result<ProductDetailsDto>>;
