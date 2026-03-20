using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.ShippingProfiles;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.DisableShippingMethod;

public sealed record DisableShippingMethodCommand(
    ShippingMethodType MethodType) : IRequest<Result>;