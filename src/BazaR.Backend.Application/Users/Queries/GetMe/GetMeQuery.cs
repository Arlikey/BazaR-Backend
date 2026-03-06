using BazaR.Backend.Application.Users.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Users.Queries.GetMe;

public sealed record GetMeQuery() : IRequest<Result<MeDto>>;