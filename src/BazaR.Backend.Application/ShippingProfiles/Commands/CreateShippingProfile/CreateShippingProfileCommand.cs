using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.CreateShippingProfile;

public sealed record CreateShippingProfileCommand : IRequest<Result<Guid>>;