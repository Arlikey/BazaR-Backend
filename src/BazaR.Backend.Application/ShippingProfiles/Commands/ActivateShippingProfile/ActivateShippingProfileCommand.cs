using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.ActivateShippingProfile;

public sealed record ActivateShippingProfileCommand : IRequest<Result>;