namespace BazaR.Backend.Application.Users.DTOs;

public sealed record MeDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    string Status,
    IReadOnlyList<string> Roles,
    string? AvatarUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset? LastLoginAt
);