using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.ActivateBrand;

public sealed record ActivateBrandCommand(Guid BrandId) : IRequest<Result>;