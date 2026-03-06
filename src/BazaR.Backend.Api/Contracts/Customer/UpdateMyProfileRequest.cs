namespace BazaR.Backend.Api.Contracts.Customer
{
    public sealed record UpdateMyProfileRequest(string FirstName, string LastName, string? Phone);
    public sealed record ChangeMyEmailRequest(string Email);
    public sealed record ChangeMyPhoneRequest(string? Phone);
}
