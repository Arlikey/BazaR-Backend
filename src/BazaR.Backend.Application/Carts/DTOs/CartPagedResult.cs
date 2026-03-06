namespace BazaR.Backend.Application.Carts.DTOs;

public sealed class CartPagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public long TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }

    public CartPagedResult(
        IReadOnlyList<T> items,
        long totalCount,
        int page,
        int pageSize)
    {
        Items = items ?? Array.Empty<T>();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}