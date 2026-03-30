using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Brands;

public static class BrandErrors
{
    public static readonly Error NameRequired = new(
        "Brand.Name.Required",
        "Brand name is required.");

    public static readonly Error NameTooLong = new(
        "Brand.Name.TooLong",
        "Brand name is too long.");

    public static readonly Error SlugRequired = new(
        "Brand.Slug.Required",
        "Brand slug is required.");

    public static readonly Error SlugTooLong = new(
        "Brand.Slug.TooLong",
        "Brand slug is too long.");

    public static readonly Error AlreadyArchived = new(
        "Brand.Status.AlreadyArchived",
        "Brand is already archived.");

    public static readonly Error AlreadyActive = new(
        "Brand.Status.AlreadyActive",
        "Brand is already active.");

    public static readonly Error CannotUseArchivedBrand = new(
        "Brand.Status.Archived",
        "Archived brand cannot be used.");
}