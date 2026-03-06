using BazaR.Backend.Application.Abstractions.Files;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Users.Commands.SetAvatar;

public sealed record SetMyAvatarCommand(UploadFile Image) : IRequest<Result>;