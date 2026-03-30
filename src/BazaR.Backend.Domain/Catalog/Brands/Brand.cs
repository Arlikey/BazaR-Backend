using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Domain.Brands;

public sealed class Brand : Entity<BrandId>
{
    private const int MaxNameLength = 150;
    private const int MaxSlugLength = 150;
    private const int MaxLogoUrlLength = 500;
    private const int MaxDescriptionLength = 2000;

    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? LogoUrl { get; private set; }
    public string? Description { get; private set; }
    public BrandStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private Brand() { }

    private Brand(
        BrandId id,
        string name,
        string slug,
        string? logoUrl,
        string? description,
        BrandStatus status,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
        : base(id)
    {
        Name = name;
        Slug = slug;
        LogoUrl = logoUrl;
        Description = description;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static Result<Brand> Create(
        string name,
        string slug,
        string? logoUrl = null,
        string? description = null)
    {
        var normalizedName = NormalizeName(name);
        if (normalizedName is null)
            return Result<Brand>.Failure(BrandErrors.NameRequired);

        if (normalizedName.Length > MaxNameLength)
            return Result<Brand>.Failure(BrandErrors.NameTooLong);

        var normalizedSlug = NormalizeSlug(slug);
        if (normalizedSlug is null)
            return Result<Brand>.Failure(BrandErrors.SlugRequired);

        if (normalizedSlug.Length > MaxSlugLength)
            return Result<Brand>.Failure(BrandErrors.SlugTooLong);

        var normalizedLogoUrl = NormalizeOptional(logoUrl);
        if (normalizedLogoUrl is not null && normalizedLogoUrl.Length > MaxLogoUrlLength)
        {
            return Result<Brand>.Failure(new Error(
                "Brand.LogoUrl.TooLong",
                "Brand logo url is too long."));
        }

        var normalizedDescription = NormalizeOptional(description);
        if (normalizedDescription is not null && normalizedDescription.Length > MaxDescriptionLength)
        {
            return Result<Brand>.Failure(new Error(
                "Brand.Description.TooLong",
                "Brand description is too long."));
        }

        var now = DateTime.UtcNow;

        return Result<Brand>.Success(new Brand(
            BrandId.New(),
            normalizedName,
            normalizedSlug,
            normalizedLogoUrl,
            normalizedDescription,
            BrandStatus.Active,
            now,
            now));
    }

    public Result Rename(string name)
    {
        var normalizedName = NormalizeName(name);
        if (normalizedName is null)
            return Result.Failure(BrandErrors.NameRequired);

        if (normalizedName.Length > MaxNameLength)
            return Result.Failure(BrandErrors.NameTooLong);

        if (Name == normalizedName)
            return Result.Success();

        Name = normalizedName;
        Touch();

        return Result.Success();
    }

    public Result ChangeSlug(string slug)
    {
        var normalizedSlug = NormalizeSlug(slug);
        if (normalizedSlug is null)
            return Result.Failure(BrandErrors.SlugRequired);

        if (normalizedSlug.Length > MaxSlugLength)
            return Result.Failure(BrandErrors.SlugTooLong);

        if (Slug == normalizedSlug)
            return Result.Success();

        Slug = normalizedSlug;
        Touch();

        return Result.Success();
    }

    public Result ChangeLogo(string? logoUrl)
    {
        var normalizedLogoUrl = NormalizeOptional(logoUrl);
        if (normalizedLogoUrl is not null && normalizedLogoUrl.Length > MaxLogoUrlLength)
        {
            return Result.Failure(new Error(
                "Brand.LogoUrl.TooLong",
                "Brand logo url is too long."));
        }

        if (LogoUrl == normalizedLogoUrl)
            return Result.Success();

        LogoUrl = normalizedLogoUrl;
        Touch();

        return Result.Success();
    }

    public Result ChangeDescription(string? description)
    {
        var normalizedDescription = NormalizeOptional(description);
        if (normalizedDescription is not null && normalizedDescription.Length > MaxDescriptionLength)
        {
            return Result.Failure(new Error(
                "Brand.Description.TooLong",
                "Brand description is too long."));
        }

        if (Description == normalizedDescription)
            return Result.Success();

        Description = normalizedDescription;
        Touch();

        return Result.Success();
    }

    public Result Archive()
    {
        if (Status == BrandStatus.Archived)
            return Result.Failure(BrandErrors.AlreadyArchived);

        Status = BrandStatus.Archived;
        Touch();

        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == BrandStatus.Active)
            return Result.Failure(BrandErrors.AlreadyActive);

        Status = BrandStatus.Active;
        Touch();

        return Result.Success();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string? NormalizeName(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeSlug(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}