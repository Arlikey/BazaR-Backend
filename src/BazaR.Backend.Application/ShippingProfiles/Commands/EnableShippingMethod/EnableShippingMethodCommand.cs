using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.ShippingProfiles;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.EnableShippingMethod;

public sealed record EnableShippingMethodCommand(
    ShippingMethodType MethodType) : IRequest<Result>;