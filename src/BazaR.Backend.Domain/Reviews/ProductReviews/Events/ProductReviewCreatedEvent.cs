using BazaR.Backend.Domain.Catalog.Products;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;

namespace BazaR.Backend.Domain.Reviews.ProductReviews.Events;

public sealed record ProductReviewCreatedEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId AuthorUserId) : DomainEvent;


public sealed record ProductReviewEditedEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId AuthorUserId) : DomainEvent;

public sealed record ProductReviewDeletedByUserEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId AuthorUserId) : DomainEvent;

public sealed record ProductReviewDeletedByAdminEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId AuthorUserId) : DomainEvent;

public sealed record ProductReviewApprovedEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId AuthorUserId) : DomainEvent;

public sealed record ProductReviewRejectedEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId AuthorUserId) : DomainEvent;

public sealed record ProductReviewVotedEvent(
    ProductReviewId ReviewId,
    ProductId ProductId,
    UserId VoterUserId,
    bool IsHelpful) : DomainEvent;