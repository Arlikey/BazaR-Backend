using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Catalog.Products.DTOs
{
    public sealed record Pagination(int Page = 1, int PageSize = 20)
    {
        public int SafePage => Page < 1 ? 1 : Page;
        public int SafePageSize => PageSize is < 1 or > 200 ? 20 : PageSize;
    }
}
