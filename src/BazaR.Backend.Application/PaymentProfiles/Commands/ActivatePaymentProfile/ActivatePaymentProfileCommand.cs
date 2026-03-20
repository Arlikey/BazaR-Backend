using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.ActivatePaymentProfile;

public sealed record ActivatePaymentProfileCommand : IRequest<Result>;