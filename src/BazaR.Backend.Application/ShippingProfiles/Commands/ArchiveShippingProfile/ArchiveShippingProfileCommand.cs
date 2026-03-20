using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.ArchiveShippingProfile;

public sealed record ArchiveShippingProfileCommand : IRequest<Result>;