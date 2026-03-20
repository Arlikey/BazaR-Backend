using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.ArchivePaymentProfile;

public sealed record ArchivePaymentProfileCommand : IRequest<Result>;