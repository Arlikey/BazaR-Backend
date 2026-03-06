namespace BazaR.Backend.Api.Contracts.Admin;

public sealed record RejectSellerRequest(string Reason);

public sealed record SuspendSellerRequest(string? Reason);
