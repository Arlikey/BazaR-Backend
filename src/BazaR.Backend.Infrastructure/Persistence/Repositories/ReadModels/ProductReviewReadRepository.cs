using BazaR.Backend.Application.Catalog.Reviews.ProductReviews.DTOs;
using BazaR.Backend.Application.Reviews.ProductReviews.Abstractions;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.ProductReviews;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class ProductReviewReadRepository : IProductReviewReadRepository
{
    private readonly AppDbContext _db;

    public ProductReviewReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ProductReviewListItemDto>> GetProductReviewsAsync(
        Guid productId,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<ProductReview> baseQuery = _db.ProductReviews
            .AsNoTracking()
            .Where(x => x.ProductId == new ProductId(productId))
            .Where(x => x.Status == ReviewStatus.Approved);

        var totalCount = await baseQuery.CountAsync(ct);

        var normalizedSort = sortBy?.Trim().ToLowerInvariant();

        IQueryable<ProductReview> query = normalizedSort switch
        {
            "oldest" => baseQuery.OrderBy(x => x.CreatedAtUtc),
            "rating_desc" => baseQuery.OrderByDescending(x => x.Rating.Value).ThenByDescending(x => x.CreatedAtUtc),
            "rating_asc" => baseQuery.OrderBy(x => x.Rating.Value).ThenByDescending(x => x.CreatedAtUtc),
            "helpful" => baseQuery.OrderByDescending(x => x.CreatedAtUtc),
            _ => baseQuery.OrderByDescending(x => x.CreatedAtUtc)
        };

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                Review = x,
                User = _db.Users
                    .Where(u => u.Id == x.AuthorUserId)
                    .Select(u => new
                    {
                        u.Id,
                        AuthorDisplayName = u.Name.FirstName + " " + u.Name.LastName
                    })
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var items = reviews
            .Select(x => new ProductReviewListItemDto
            {
                ReviewId = x.Review.Id.Value,
                ProductId = x.Review.ProductId.Value,
                AuthorUserId = x.Review.AuthorUserId.Value,
                AuthorDisplayName = x.User != null ? x.User.AuthorDisplayName : null,
                Rating = x.Review.Rating.Value,
                Advantages = x.Review.Advantages,
                Disadvantages = x.Review.Disadvantages,
                Body = x.Review.Body,
                Status = x.Review.Status.ToString(),
                HelpfulVotesCount = x.Review.Votes.Count(v => v.IsHelpful),
                NotHelpfulVotesCount = x.Review.Votes.Count(v => !v.IsHelpful),
                CreatedAtUtc = x.Review.CreatedAtUtc,
                UpdatedAtUtc = x.Review.UpdatedAtUtc,
                ModeratedAtUtc = x.Review.ModeratedAtUtc
            })
            .ToList();

        if (normalizedSort == "helpful")
        {
            items = items
                .OrderByDescending(x => x.HelpfulVotesCount)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToList();
        }

        return new PagedResult<ProductReviewListItemDto>(
            items,
            totalCount,
            page,
            pageSize);
    }

    public async Task<ProductReviewDetailsDto?> GetByIdAsync(
        Guid reviewId,
        CancellationToken ct = default)
    {
        var review = await _db.ProductReviews
            .AsNoTracking()
            .Where(x => x.Id == new ProductReviewId(reviewId))
            .Select(x => new
            {
                Review = x,
                User = _db.Users
                    .Where(u => u.Id == x.AuthorUserId)
                    .Select(u => new
                    {
                        u.Id,
                        AuthorDisplayName = u.Name.FirstName + " " + u.Name.LastName
                    })
                    .FirstOrDefault()
            })
            .SingleOrDefaultAsync(ct);

        if (review is null)
            return null;

        return new ProductReviewDetailsDto
        {
            ReviewId = review.Review.Id.Value,
            ProductId = review.Review.ProductId.Value,
            AuthorUserId = review.Review.AuthorUserId.Value,
            AuthorDisplayName = review.User != null ? review.User.AuthorDisplayName : null,
            Rating = review.Review.Rating.Value,
            Advantages = review.Review.Advantages,
            Disadvantages = review.Review.Disadvantages,
            Body = review.Review.Body,
            Status = review.Review.Status.ToString(),
            HelpfulVotesCount = review.Review.Votes.Count(v => v.IsHelpful),
            NotHelpfulVotesCount = review.Review.Votes.Count(v => !v.IsHelpful),
            CreatedAtUtc = review.Review.CreatedAtUtc,
            UpdatedAtUtc = review.Review.UpdatedAtUtc,
            ModeratedAtUtc = review.Review.ModeratedAtUtc
        };
    }

    public async Task<ProductReviewSummaryDto?> GetSummaryAsync(
        Guid productId,
        CancellationToken ct = default)
    {
        return await _db.ProductRatingSummaries
            .AsNoTracking()
            .Where(x => x.Id == new ProductId(productId))
            .Select(x => new ProductReviewSummaryDto
            {
                ProductId = x.Id.Value,
                AverageRating = x.AverageRating,
                ReviewsCount = x.ReviewsCount,
                FiveStarsCount = x.FiveStarsCount,
                FourStarsCount = x.FourStarsCount,
                ThreeStarsCount = x.ThreeStarsCount,
                TwoStarsCount = x.TwoStarsCount,
                OneStarCount = x.OneStarCount
            })
            .SingleOrDefaultAsync(ct);
    }

    public async Task<PagedResult<ProductReviewListItemDto>> GetForModerationAsync(
        string? status,
        Guid? productId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<ProductReview> query = _db.ProductReviews
            .AsNoTracking();

        if (productId.HasValue)
        {
            query = query.Where(x => x.ProductId == new ProductId(productId.Value));
        }

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<ReviewStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        query = query.OrderByDescending(x => x.CreatedAtUtc);

        var totalCount = await query.CountAsync(ct);

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                Review = x,
                User = _db.Users
                    .Where(u => u.Id == x.AuthorUserId)
                    .Select(u => new
                    {
                        u.Id,
                        AuthorDisplayName = u.Name.FirstName + " " + u.Name.LastName
                    })
                    .FirstOrDefault()
            })
            .ToListAsync(ct);

        var items = reviews
            .Select(x => new ProductReviewListItemDto
            {
                ReviewId = x.Review.Id.Value,
                ProductId = x.Review.ProductId.Value,
                AuthorUserId = x.Review.AuthorUserId.Value,
                AuthorDisplayName = x.User != null ? x.User.AuthorDisplayName : null,
                Rating = x.Review.Rating.Value,
                Advantages = x.Review.Advantages,
                Disadvantages = x.Review.Disadvantages,
                Body = x.Review.Body,
                Status = x.Review.Status.ToString(),
                HelpfulVotesCount = x.Review.Votes.Count(v => v.IsHelpful),
                NotHelpfulVotesCount = x.Review.Votes.Count(v => !v.IsHelpful),
                CreatedAtUtc = x.Review.CreatedAtUtc,
                UpdatedAtUtc = x.Review.UpdatedAtUtc,
                ModeratedAtUtc = x.Review.ModeratedAtUtc
            })
            .ToList();

        return new PagedResult<ProductReviewListItemDto>(
            items,
            totalCount,
            page,
            pageSize);
    }
}