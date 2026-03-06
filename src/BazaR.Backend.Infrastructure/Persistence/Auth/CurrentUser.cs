using System.Security.Claims;
using BazaR.Backend.Application.Common.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BazaR.Backend.Infrastructure.Auth;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _ctx;

    public CurrentUser(IHttpContextAccessor ctx) => _ctx = ctx;

    public bool IsAuthenticated => _ctx.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public Guid UserId
    {
        get
        {
            var raw = _ctx.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }
    }

    public IReadOnlyCollection<string> Roles
        => _ctx.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList()
           ?? new List<string>();

    public bool IsAdmin => Roles.Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
}
