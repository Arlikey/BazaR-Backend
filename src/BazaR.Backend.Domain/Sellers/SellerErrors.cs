using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Sellers;

public static class SellerErrors
{
    public static readonly Error NotFound =
        new("Seller.NotFound", "Seller was not found.");

    public static readonly Error NameRequired =
        new("Seller.NameRequired", "Seller name is required.");

    public static readonly Error NameTooLong =
        new("Seller.NameTooLong", "Seller name is too long.");

    public static readonly Error InvalidStatusTransition =
        new("Seller.InvalidStatusTransition", "Invalid seller status transition.");

    public static readonly Error CannotActivateWithoutApproval =
        new("Seller.CannotActivateWithoutApproval", "Seller cannot be activated without approval.");

    public static readonly Error CannotModifyClosed =
        new("Seller.CannotModifyClosed", "Closed seller cannot be modified.");
    public static readonly Error SlugAlreadyTaken = new("Seller.SlugTaken", "Slug is already taken.");
    public static readonly Error TaxNumberAlreadyExists = new("Seller.TaxNumberTaken", "Tax number already exists.");
   
}
