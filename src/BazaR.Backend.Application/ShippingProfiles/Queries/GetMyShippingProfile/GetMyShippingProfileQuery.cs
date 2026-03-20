using BazaR.Backend.Application.ShippingProfiles.DTOs;

using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Queries.GetMyShippingProfile;

public sealed record GetMyShippingProfileQuery : IRequest<Result<ShippingProfileDto>>;