using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Catalog.Brands.Commands.ArchiveBrand;

public sealed record ArchiveBrandCommand(Guid BrandId) : IRequest<Result>;