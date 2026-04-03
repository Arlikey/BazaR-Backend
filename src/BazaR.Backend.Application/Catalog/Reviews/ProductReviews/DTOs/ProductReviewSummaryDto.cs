using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs
{
    public sealed class ProductReviewSummaryDto
    {
        public Guid ProductId { get; init; }
        public decimal AverageRating { get; init; }
        public int ReviewsCount { get; init; }
        public int FiveStarsCount { get; init; }
        public int FourStarsCount { get; init; }
        public int ThreeStarsCount { get; init; }
        public int TwoStarsCount { get; init; }
        public int OneStarCount { get; init; }
    }
}
