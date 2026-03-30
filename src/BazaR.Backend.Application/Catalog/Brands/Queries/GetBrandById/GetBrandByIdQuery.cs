using BazaR.Backend.Application.Abstractions.ReadModels;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Queries.GetBrandById;

public sealed record GetBrandByIdQuery(Guid BrandId) : IRequest<BrandDetailsDto?>;