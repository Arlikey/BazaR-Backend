using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Commands.ProcessLiqPayCallback;

public sealed record ProcessLiqPayCallbackCommand(
    string Data,
    string Signature) : IRequest<Result>;