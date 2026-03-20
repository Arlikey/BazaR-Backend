using BazaR.Backend.Application.PaymentProfiles.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Queries.GetMyPaymentProfile;

public sealed record GetMyPaymentProfileQuery : IRequest<Result<PaymentProfileDto>>;