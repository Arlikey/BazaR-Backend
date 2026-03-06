namespace BazaR.Backend.Application.Carts.DTOs;

public sealed record Pagination(int Page = 1, int PageSize = 20)
{
    public int SafePage => Page < 1 ? 1 : Page;
    public int SafePageSize => PageSize switch
    {
        < 1 => 20,
        > 200 => 200,
        _ => PageSize
    };
}