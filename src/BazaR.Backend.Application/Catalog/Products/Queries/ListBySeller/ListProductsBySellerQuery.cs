using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Catalog.Products.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Products.Queries.ListBySeller;

public sealed record ListProductsBySellerQuery(Guid SellerId, int Limit)
    : IRequest<Result<IReadOnlyList<ProductCardWithOfferDto>>>;