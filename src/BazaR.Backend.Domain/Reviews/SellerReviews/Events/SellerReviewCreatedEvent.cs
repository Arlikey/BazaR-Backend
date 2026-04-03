using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.SellerReviews.Events;

public sealed record SellerReviewCreatedEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId AuthorUserId) : DomainEvent;

public sealed record SellerReviewEditedEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId AuthorUserId) : DomainEvent;

public sealed record SellerReviewDeletedByUserEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId AuthorUserId) : DomainEvent;

public sealed record SellerReviewDeletedByAdminEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId AuthorUserId) : DomainEvent;

public sealed record SellerReviewApprovedEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId AuthorUserId) : DomainEvent;

public sealed record SellerReviewRejectedEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId AuthorUserId) : DomainEvent;

public sealed record SellerReviewVotedEvent(
    SellerReviewId ReviewId,
    SellerId SellerId,
    UserId VoterUserId,
    bool IsHelpful) : DomainEvent;