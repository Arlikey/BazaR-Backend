namespace BazaR.Backend.Application.Shippings.DTOs;

public sealed record ShippingListFilter(
    string? Query,
    int? Method,
    int? Status,
    int Page = 1,
    int PageSize = 20);