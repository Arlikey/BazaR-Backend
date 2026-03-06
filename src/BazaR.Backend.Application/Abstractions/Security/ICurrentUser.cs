namespace BazaR.Backend.Application.Common.Abstractions;

public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    IReadOnlyCollection<string> Roles { get; }
}
