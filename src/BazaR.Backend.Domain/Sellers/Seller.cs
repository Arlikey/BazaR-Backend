using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers.Events;

namespace BazaR.Backend.Domain.Sellers;

public sealed class Seller : AggregateRoot<SellerId>
{
    private const int MaxNameLength = 200;
    private const int MaxLegalNameLength = 300;
    private const int MaxTaxNumberLength = 32;
    private const int MaxDescriptionLength = 2000;
    private const int MaxCloseReasonLength = 500;

    public string Name { get; private set; } = default!;
    public SellerSlug Slug { get; private set; } = default!;
    public SellerType Type { get; private set; }
    public SellerStatus Status { get; private set; }

    // ownership
    public Guid? OwnerUserId { get; private set; }

    // storefront public info
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }

    // business/legal
    public string? LegalName { get; private set; }
    public string? TaxNumber { get; private set; }
    public CountryCode CountryCode { get; private set; } = default!;

    // contacts
    public EmailAddress? SupportEmail { get; private set; }
    public PhoneNumber? SupportPhone { get; private set; }

    // moderation decision history
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? LastDecisionAt { get; private set; }
    public Guid? LastDecisionBy { get; private set; }
    public string? LastRejectionReason { get; private set; }

    // suspension
    public string? SuspensionReason { get; private set; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public Guid? SuspendedBy { get; private set; }

    // closing
    public DateTimeOffset? ClosedAt { get; private set; }
    public Guid? ClosedBy { get; private set; }
    public string? CloseReason { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Seller(
        SellerId id,
        string name,
        SellerSlug slug,
        SellerType type,
        Guid? ownerUserId,
        CountryCode country,
        DateTimeOffset nowUtc) : base(id)
    {
        Name = name;
        Slug = slug;
        Type = type;
        OwnerUserId = ownerUserId;
        CountryCode = country;

        Status = type == SellerType.Platform ? SellerStatus.Active : SellerStatus.Draft;

        CreatedAt = nowUtc;
        UpdatedAt = nowUtc;

        AddDomainEvent(new SellerCreatedEvent(Id));
    }

    private Seller() { } 

    public static Result<Seller> Create(
        string name,
        string slug,
        SellerType type = SellerType.Regular,
        Guid? ownerUserId = null,
        string countryCode = "UA",
        DateTimeOffset? nowUtc = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Seller>.Failure(SellerErrors.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength)
            return Result<Seller>.Failure(SellerErrors.NameTooLong);

        var slugRes = SellerSlug.Create(slug);
        if (slugRes.IsFailure) return Result<Seller>.Failure(slugRes.Error);

        var ccRes = CountryCode.Create(countryCode);
        if (ccRes.IsFailure) return Result<Seller>.Failure(ccRes.Error);

        
        if (type != SellerType.Platform && (ownerUserId is null || ownerUserId == Guid.Empty))
            return Result<Seller>.Failure(new Error("Seller.OwnerRequired", "Owner user id is required."));

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        return Result<Seller>.Success(
            new Seller(SellerId.New(), trimmed, slugRes.Value!, type, ownerUserId, ccRes.Value!, now));
    }

    private void Touch(DateTimeOffset? nowUtc = null)
        => UpdatedAt = nowUtc ?? DateTimeOffset.UtcNow;

    private Result EnsureNotClosed()
        => Status == SellerStatus.Closed
            ? Result.Failure(SellerErrors.CannotModifyClosed)
            : Result.Success();

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    private Result EnsureReadyForApproval()
    {
        if (SupportEmail is null && SupportPhone is null)
            return Result.Failure(new Error("Seller.ContactsRequired", "Support contacts are required."));
        return Result.Success();
    }

    // -------------------------
    // Public storefront
    // -------------------------
    public Result UpdateProfile(string? description, string? logoUrl)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        var d = Normalize(description);
        if (d is not null && d.Length > MaxDescriptionLength)
            return Result.Failure(new Error("Seller.DescriptionTooLong", "Description is too long."));

        Description = d;
        LogoUrl = Normalize(logoUrl);

        Touch();
        return Result.Success();
    }

    public Result Rename(string name)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(SellerErrors.NameRequired);

        var trimmed = name.Trim();
        if (trimmed.Length > MaxNameLength)
            return Result.Failure(SellerErrors.NameTooLong);

        if (Name == trimmed) return Result.Success();

        Name = trimmed;
        Touch();
        return Result.Success();
    }

    public Result ChangeSlug(string slug)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        var slugRes = SellerSlug.Create(slug);
        if (slugRes.IsFailure) return slugRes;

        if (Slug == slugRes.Value!) return Result.Success();

        Slug = slugRes.Value!;
        Touch();
        AddDomainEvent(new SellerSlugChangedEvent(Id, Slug.Value));
        return Result.Success();
    }

    public Result UpdateLegal(string? legalName, string? taxNumber, string? countryCode)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        var ln = Normalize(legalName);
        if (ln is not null && ln.Length > MaxLegalNameLength)
            return Result.Failure(new Error("Seller.LegalNameTooLong", "Legal name is too long."));

        var tn = Normalize(taxNumber);
        if (tn is not null && tn.Length > MaxTaxNumberLength)
            return Result.Failure(new Error("Seller.TaxNumberTooLong", "Tax number is too long."));

        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            var ccRes = CountryCode.Create(countryCode);
            if (ccRes.IsFailure) return ccRes;
            CountryCode = ccRes.Value!;
        }

        LegalName = ln;
        TaxNumber = tn;

        Touch();
        return Result.Success();
    }

    public Result UpdateSupportContacts(string? email, string? phone)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (string.IsNullOrWhiteSpace(email))
            SupportEmail = null;
        else
        {
            var emailRes = EmailAddress.Create(email);
            if (emailRes.IsFailure)
                return Result.Failure(new Error("Seller.EmailInvalid", "Email format is invalid."));
            SupportEmail = emailRes.Value!;
        }

        if (string.IsNullOrWhiteSpace(phone))
            SupportPhone = null;
        else
        {
            var phoneRes = PhoneNumber.Create(phone);
            if (phoneRes.IsFailure)
                return Result.Failure(new Error("Seller.PhoneInvalid", "Phone format is invalid."));
            SupportPhone = phoneRes.Value!;
        }

        Touch();
        return Result.Success();
    }

    // =========================
    // FSM / Moderation
    // =========================

    public Result SubmitForApproval(DateTimeOffset? nowUtc = null)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (Type == SellerType.Platform)
            return Result.Success();

        if (Status != SellerStatus.Draft)
            return Result.Failure(SellerErrors.InvalidStatusTransition);

        var ready = EnsureReadyForApproval();
        if (ready.IsFailure) return ready;

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = SellerStatus.PendingApproval;
        SubmittedAt = now;
        Touch(now);

        AddDomainEvent(new SellerSubmittedForApprovalEvent(Id));
        return Result.Success();
    }

    public Result Approve(Guid adminUserId, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (Type == SellerType.Platform)
            return Result.Failure(new Error("Seller.PlatformNoApproval", "Platform seller does not require approval."));

        if (Status != SellerStatus.PendingApproval)
            return Result.Failure(SellerErrors.InvalidStatusTransition);

        if (OwnerUserId is null)
            return Result.Failure(new Error("Seller.OwnerRequired", "OwnerUserId is required to approve seller."));

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = SellerStatus.Active;
        LastDecisionAt = now;
        LastDecisionBy = adminUserId;
        LastRejectionReason = null;
        Touch(now);

        AddDomainEvent(new SellerApprovedEvent(Id, OwnerUserId.Value, adminUserId, now));
        return Result.Success();
    }


    public Result Reject(Guid adminUserId, string reason, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (Type == SellerType.Platform)
            return Result.Failure(new Error("Seller.PlatformNoApproval", "Platform seller does not require approval."));

        if (Status != SellerStatus.PendingApproval)
            return Result.Failure(SellerErrors.InvalidStatusTransition);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = SellerStatus.Draft;
        LastDecisionAt = now;
        LastDecisionBy = adminUserId;
        LastRejectionReason = string.IsNullOrWhiteSpace(reason) ? "Rejected" : reason.Trim();
        Touch(now);

        AddDomainEvent(new SellerRejectedEvent(Id, adminUserId, LastRejectionReason));
        return Result.Success();
    }

    public Result Suspend(Guid adminUserId, string? reason = null, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (Status != SellerStatus.Active)
            return Result.Failure(SellerErrors.InvalidStatusTransition);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = SellerStatus.Suspended;
        SuspensionReason = Normalize(reason);
        SuspendedAt = now;
        SuspendedBy = adminUserId;
        Touch(now);

        AddDomainEvent(new SellerSuspendedEvent(Id, SuspensionReason));
        return Result.Success();
    }

    public Result Reactivate(Guid adminUserId, DateTimeOffset? nowUtc = null)
    {
        var g = EnsureNotClosed();
        if (g.IsFailure) return g;

        if (Status != SellerStatus.Suspended)
            return Result.Failure(SellerErrors.InvalidStatusTransition);

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = SellerStatus.Active;
        SuspensionReason = null;
        SuspendedAt = null;
        SuspendedBy = null;
        Touch(now);

        AddDomainEvent(new SellerReactivatedEvent(Id));
        return Result.Success();
    }

    public Result Close(Guid adminUserId, string? reason = null, DateTimeOffset? nowUtc = null)
    {
        if (Status == SellerStatus.Closed)
            return Result.Success();

        var now = nowUtc ?? DateTimeOffset.UtcNow;

        Status = SellerStatus.Closed;
        ClosedAt = now;
        ClosedBy = adminUserId;

        var r = Normalize(reason);
        if (r is not null && r.Length > MaxCloseReasonLength)
            return Result.Failure(new Error("Seller.CloseReasonTooLong", "Close reason is too long."));

        CloseReason = r;
        Touch(now);

        AddDomainEvent(new SellerClosedEvent(Id));
        return Result.Success();
    }
}
