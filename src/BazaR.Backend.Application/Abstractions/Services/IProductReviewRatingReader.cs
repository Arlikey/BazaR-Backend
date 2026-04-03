using BazaR.Backend.Domain.Catalog.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Abstractions.Services
{
    public interface IProductReviewRatingReader
    {
        Task<IReadOnlyList<int>> GetApprovedRatingsAsync(
            ProductId productId,
            CancellationToken ct = default);
    }
}
