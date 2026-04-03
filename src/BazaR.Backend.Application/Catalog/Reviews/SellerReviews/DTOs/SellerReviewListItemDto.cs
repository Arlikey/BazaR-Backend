using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs
{
    public sealed class SellerReviewListItemDto
    {
        public Guid ReviewId { get; init; }
        public Guid SellerId { get; init; }
        public Guid AuthorUserId { get; init; }
        public string AuthorDisplayName { get; init; } = default!;
        public int Rating { get; init; }
        public string Title { get; init; } = default!;
        public string Body { get; init; } = default!;
        public string Status { get; init; } = default!;
        public int HelpfulVotesCount { get; init; }
        public int NotHelpfulVotesCount { get; init; }
        public DateTime CreatedAtUtc { get; init; }
        public DateTime UpdatedAtUtc { get; init; }
        public DateTime? ModeratedAtUtc { get; set; }
    }
}
