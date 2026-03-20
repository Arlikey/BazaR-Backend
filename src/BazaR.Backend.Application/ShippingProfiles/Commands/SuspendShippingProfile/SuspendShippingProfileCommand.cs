using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.SuspendShippingProfile;

public sealed record SuspendShippingProfileCommand : IRequest<Result>;