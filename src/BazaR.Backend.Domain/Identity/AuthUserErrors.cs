using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Identity;

public static class AuthUserErrors
{
    public static readonly Error NotFound = new("Identity.NotFound", "Auth user not found.");
    public static readonly Error EmailTaken = new("Identity.EmailTaken", "Email is already taken.");
    public static readonly Error InvalidCredentials = new("Identity.InvalidCredentials", "Invalid credentials.");

    public static readonly Error RefreshTokenInvalid = new("Identity.RefreshTokenInvalid", "Refresh token is invalid.");
    public static readonly Error RefreshTokenExpired = new("Identity.RefreshTokenExpired", "Refresh token is expired.");
    public static readonly Error RefreshTokenRevoked = new("Identity.RefreshTokenRevoked", "Refresh token is revoked.");
}
