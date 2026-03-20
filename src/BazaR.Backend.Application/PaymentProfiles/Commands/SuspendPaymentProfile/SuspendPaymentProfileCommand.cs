using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.SuspendPaymentProfile;

public sealed record SuspendPaymentProfileCommand : IRequest<Result>;