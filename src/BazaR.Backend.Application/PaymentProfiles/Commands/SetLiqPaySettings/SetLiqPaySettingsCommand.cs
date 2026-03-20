using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.SetLiqPaySettings;

public sealed record SetLiqPaySettingsCommand(
    string PublicKey,
    string PrivateKey,
    string? ResultUrl,
    string? ServerCallbackUrl,
    bool CheckoutEnabled,
    bool PrivatPayEnabled,
    bool InstallmentsEnabled) : IRequest<Result>;