using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Reviews;

public static class ReviewErrors
{
    public static readonly Error NotFound = new(
        "Review.NotFound",
        "Review was not found.");

    public static readonly Error TitleRequired = new(
        "Review.Title.Required",
        "Review title is required.");

    public static readonly Error TitleTooLong = new(
        "Review.Title.TooLong",
        "Review title is too long.");

    public static readonly Error BodyRequired = new(
        "Review.Body.Required",
        "Review body is required.");

    public static readonly Error BodyTooLong = new(
        "Review.Body.TooLong",
        "Review body is too long.");

    public static readonly Error AlreadyDeleted = new(
        "Review.Status.AlreadyDeleted",
        "Review is already deleted.");

    public static readonly Error CannotEditDeleted = new(
        "Review.Edit.Deleted",
        "Deleted review cannot be edited.");

    public static readonly Error CannotVoteDeleted = new(
        "Review.Vote.Deleted",
        "Deleted review cannot be voted.");

    public static readonly Error CannotModerateDeleted = new(
        "Review.Moderation.Deleted",
        "Deleted review cannot be moderated.");

    public static readonly Error CannotVoteOwnReview = new(
        "Review.Vote.OwnReview",
        "You cannot vote for your own review.");

    public static readonly Error DuplicateProductReview = new(
        "ProductReview.Duplicate",
        "User has already left a review for this product.");

    public static readonly Error DuplicateSellerReview = new(
        "SellerReview.Duplicate",
        "User has already left a review for this seller.");

    public static readonly Error ProductRequired = new(
        "ProductReview.Product.Required",
        "Product is required.");

    public static readonly Error SellerRequired = new(
        "SellerReview.Seller.Required",
        "Seller is required.");

    public static readonly Error AuthorRequired = new(
        "Review.Author.Required",
        "Author is required.");


}