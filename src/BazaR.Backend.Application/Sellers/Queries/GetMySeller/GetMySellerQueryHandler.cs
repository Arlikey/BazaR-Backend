using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Sellers.DTOs;

using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Queries.GetMySeller;

public sealed class GetMySellerQueryHandler : IRequestHandler<GetMySellerQuery, Result<SellerDto>>
{
    private readonly ISellerRepository _sellers;
    private readonly ICurrentUser _current;

    public GetMySellerQueryHandler(ISellerRepository sellers, ICurrentUser current)
    {
        _sellers = sellers;
        _current = current;
    }

    public async Task<Result<SellerDto>> Handle(GetMySellerQuery request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<SellerDto>.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<SellerDto>.Failure(new Error("Seller.NotFound", "Seller not found."));

        return Result<SellerDto>.Success(Map(seller));
    }

    private static SellerDto Map(BazaR.Backend.Domain.Sellers.Seller s)
        => new(
            Id: s.Id.Value,
            Name: s.Name,
            Slug: s.Slug.Value,
            Type: s.Type,
            Status: s.Status,
            OwnerUserId: s.OwnerUserId,
            LegalName: s.LegalName,
            TaxNumber: s.TaxNumber,
            CountryCode: s.CountryCode.Value,
            SupportEmail: s.SupportEmail?.Value,
            SupportPhone: s.SupportPhone?.Value,
            CreatedAt: s.CreatedAt,
            LastDecisionAt: s.LastDecisionAt,
            LastDecisionBy: s.LastDecisionBy,
            LastRejectionReason: s.LastRejectionReason,
            SuspensionReason: s.SuspensionReason
        );
}
