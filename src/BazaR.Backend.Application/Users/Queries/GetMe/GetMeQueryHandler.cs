using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Users.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.Users.Queries.GetMe;

public sealed class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<MeDto>>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUser _current;

    public GetMeQueryHandler(IUserRepository users, ICurrentUser current)
    {
        _users = users;
        _current = current;
    }

    public async Task<Result<MeDto>> Handle(GetMeQuery request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<MeDto>.Failure(new Error("Auth.Required", "Authentication required."));

        var userId = new UserId(_current.UserId);

        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
            return Result<MeDto>.Failure(new Error("User.NotFound", "User not found."));

        var dto = new MeDto(
            Id: user.Id.Value,
            Email: user.Email.Value,
            FirstName: user.Name.FirstName,
            LastName: user.Name.LastName,
            Phone: user.Phone?.Value,
            Status: user.Status.ToString(),
            Roles: user.Roles.Select(r => r.ToString()).ToArray(),
            AvatarUrl: user.GetAvatarUrl(),
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt,
            LastLoginAt: user.LastLoginAt
        );

        return Result<MeDto>.Success(dto);
    }
}