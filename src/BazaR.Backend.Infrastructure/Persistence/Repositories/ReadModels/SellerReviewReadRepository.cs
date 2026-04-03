using BazaR.Backend.Application.Catalog.Reviews.SellerReviews.DTOs;
using BazaR.Backend.Application.Reviews.SellerReviews.Abstractions;
using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Reviews;
using BazaR.Backend.Domain.Reviews.SellerReviews;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence.ReadModels;

public sealed class SellerReviewReadRepository : ISellerReviewReadRepository
{
    private readonly AppDbContext _db;

    public SellerReviewReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<SellerReviewListItemDto>> GetSellerReviewsAsync(
        Guid sellerId,
        int page,
        int pageSize,
        string? sortBy,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<SellerReview> baseQuery = _db.SellerReviews
            .AsNoTracking()
            .Where(x => x.SellerId == new SellerId(sellerId))
            .Where(x => x.Status == ReviewStatus.Approved);

        var totalCount = await baseQuery.CountAsync(ct);

        var normalizedSort = sortBy?.Trim().ToLowerInvariant();

        IQueryable<SellerReview> query = normalizedSort switch
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
            .Select(x => new SellerReviewListItemDto
            {
                ReviewId = x.Review.Id.Value,
                SellerId = x.Review.SellerId.Value,
                AuthorUserId = x.Review.AuthorUserId.Value,
                AuthorDisplayName = x.User != null ? x.User.AuthorDisplayName : null,
                Rating = x.Review.Rating.Value,
                Title = x.Review.Title,
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

        return new PagedResult<SellerReviewListItemDto>(
            items,
            totalCount,
            page,
            pageSize);
    }

    public async Task<SellerReviewDetailsDto?> GetByIdAsync(
        Guid reviewId,
        CancellationToken ct = default)
    {
        var review = await _db.SellerReviews
            .AsNoTracking()
            .Where(x => x.Id == new SellerReviewId(reviewId))
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

        return new SellerReviewDetailsDto
        {
            ReviewId = review.Review.Id.Value,
            SellerId = review.Review.SellerId.Value,
            AuthorUserId = review.Review.AuthorUserId.Value,
            AuthorDisplayName = review.User != null ? review.User.AuthorDisplayName : null,
            Rating = review.Review.Rating.Value,
            Title = review.Review.Title,
            Body = review.Review.Body,
            Status = review.Review.Status.ToString(),
            HelpfulVotesCount = review.Review.Votes.Count(v => v.IsHelpful),
            NotHelpfulVotesCount = review.Review.Votes.Count(v => !v.IsHelpful),
            CreatedAtUtc = review.Review.CreatedAtUtc,
            UpdatedAtUtc = review.Review.UpdatedAtUtc,
            ModeratedAtUtc = review.Review.ModeratedAtUtc
        };
    }

    public async Task<SellerReviewSummaryDto?> GetSummaryAsync(
        Guid sellerId,
        CancellationToken ct = default)
    {
        return await _db.SellerRatingSummaries
            .AsNoTracking()
            .Where(x => x.Id == new SellerId(sellerId))
            .Select(x => new SellerReviewSummaryDto
            {
                SellerId = x.Id.Value,
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

    public async Task<PagedResult<SellerReviewListItemDto>> GetForModerationAsync(
        string? status,
        Guid? sellerId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        IQueryable<SellerReview> query = _db.SellerReviews
            .AsNoTracking();

        if (sellerId.HasValue)
        {
            query = query.Where(x => x.SellerId == new SellerId(sellerId.Value));
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
            .Select(x => new SellerReviewListItemDto
            {
                ReviewId = x.Review.Id.Value,
                SellerId = x.Review.SellerId.Value,
                AuthorUserId = x.Review.AuthorUserId.Value,
                AuthorDisplayName = x.User != null ? x.User.AuthorDisplayName : null,
                Rating = x.Review.Rating.Value,
                Title = x.Review.Title,
                Body = x.Review.Body,
                Status = x.Review.Status.ToString(),
                HelpfulVotesCount = x.Review.Votes.Count(v => v.IsHelpful),
                NotHelpfulVotesCount = x.Review.Votes.Count(v => !v.IsHelpful),
                CreatedAtUtc = x.Review.CreatedAtUtc,
                UpdatedAtUtc = x.Review.UpdatedAtUtc,
                ModeratedAtUtc = x.Review.ModeratedAtUtc
            })
            .ToList();

        return new PagedResult<SellerReviewListItemDto>(
            items,
            totalCount,
            page,
            pageSize);
    }
}