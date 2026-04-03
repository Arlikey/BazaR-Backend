using BazaR.Backend.Domain.Sellers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Abstractions.Services
{
    public interface ISellerReviewRatingReader
    {
        Task<IReadOnlyList<int>> GetApprovedRatingsAsync(
            SellerId sellerId,
            CancellationToken ct = default);
    }
}
