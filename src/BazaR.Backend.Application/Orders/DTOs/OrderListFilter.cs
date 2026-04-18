namespace BazaR.Backend.Application.Orders.DTOs;

public sealed record OrderListFilter(
    string? Query,
    string? Tab,
    int Page = 1,
    int PageSize = 20);