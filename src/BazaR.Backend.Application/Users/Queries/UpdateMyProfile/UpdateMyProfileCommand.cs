using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileCommand(
    string FirstName,
    string LastName,
    string? Phone
) : IRequest<Result>;