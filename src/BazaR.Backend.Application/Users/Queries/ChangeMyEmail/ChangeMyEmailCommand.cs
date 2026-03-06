using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.ChangeMyEmail;

public sealed record ChangeMyEmailCommand(string Email) : IRequest<Result>;