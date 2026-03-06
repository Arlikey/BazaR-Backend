using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Carts.Commands.Clear;

public sealed record ClearCartCommand() : IRequest<Result>;