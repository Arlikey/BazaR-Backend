using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.UpdateMySellerLegal;

public sealed record UpdateMySellerLegalCommand(
    string? LegalName,
    string? TaxNumber,
    string? CountryCode
) : IRequest<Result>;
