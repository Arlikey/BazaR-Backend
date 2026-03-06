using BazaR.Backend.Domain.Sellers;
using MediatR;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Application.Sellers.Commands.CreateSeller;

public sealed record CreateSellerCommand(
    string Name,
    string Slug,
    SellerType Type,
    string CountryCode,

    // storefront
    string? Description,
    string? LogoUrl,

    // legal
    string? LegalName,
    string? TaxNumber,

    // support contacts
    string? SupportEmail,
    string? SupportPhone,

    // optional: сразу отправить на модерацию
    bool SubmitForApproval = false
) : IRequest<Result<Guid>>;
