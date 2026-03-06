namespace BazaR.Backend.Api.Contracts.Sellers;



public sealed record CreateSellerRequest(
    string Name,
    string Slug,
    string CountryCode = "UA",

   
    string? Description = null,
    string? LogoUrl = null,


    string? LegalName = null,
    string? TaxNumber = null,

  
    string? SupportEmail = null,
    string? SupportPhone = null,

    bool SubmitForApproval = true
);


public sealed record RenameSellerRequest(string Name);

public sealed record ChangeSellerSlugRequest(string Slug);

public sealed record UpdateSellerLegalRequest(string? LegalName, string? TaxNumber, string? CountryCode);

public sealed record UpdateSellerContactsRequest(string? Email, string? Phone);
