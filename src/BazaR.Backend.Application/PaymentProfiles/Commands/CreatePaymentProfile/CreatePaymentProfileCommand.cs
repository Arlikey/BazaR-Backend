using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.CreatePaymentProfile;

public sealed record CreatePaymentProfileCommand : IRequest<Result<Guid>>;