using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.ChangeMyPhone;

public sealed record ChangeMyPhoneCommand(string? Phone) : IRequest<Result>;