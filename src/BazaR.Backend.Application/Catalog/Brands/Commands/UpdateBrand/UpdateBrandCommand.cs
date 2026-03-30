using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.UpdateBrand;

public sealed record UpdateBrandCommand(
    Guid BrandId,
    string Name,
    string Slug,
    string? LogoUrl,
    string? Description) : IRequest<Result>;