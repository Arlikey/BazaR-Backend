using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.GetAttributesView;

public sealed record GetProductAttributesViewQuery(ProductId ProductId)
    : IRequest<Result<ProductAttributesViewDto>>;
