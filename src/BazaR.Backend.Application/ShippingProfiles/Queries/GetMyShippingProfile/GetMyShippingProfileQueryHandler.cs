using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.ShippingProfiles.DTOs;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Queries.GetMyShippingProfile;

public sealed class GetMyShippingProfileQueryHandler
    : IRequestHandler<GetMyShippingProfileQuery, Result<ShippingProfileDto>>
{
    private readonly IShippingProfileRepository _profiles;
    private readonly ISellerRepository _sellers;

    public GetMyShippingProfileQueryHandler(
        IShippingProfileRepository profiles,
        ISellerRepository sellers)
    {
        _profiles = profiles;
        _sellers = sellers;
    }

    public async Task<Result<ShippingProfileDto>> Handle(
        GetMyShippingProfileQuery request,
        CancellationToken ct)
    {
        var sellerId = new SellerId(request.SellerId);

        var seller = await _sellers.GetByIdAsync(sellerId, ct);
        if (seller is null)
        {
            return Result<ShippingProfileDto>.Failure(
                new Error("Seller.NotFound", "Seller was not found."));
        }

        var profile = await _profiles.GetBySellerIdAsync(seller.Id, ct);
        if (profile is null)
        {
            return Result<ShippingProfileDto>.Failure(
                new Error("ShippingProfile.NotFound", "Shipping profile was not found."));
        }

        var dto = new ShippingProfileDto(
            profile.Id.Value,
            profile.SellerId.Value,
            profile.Status,
            profile.CreatedAtUtc,
            profile.UpdatedAtUtc,
            profile.Methods
                .Select(x => new ShippingMethodDto(
                    x.MethodType,
                    x.IsEnabled,
                    x.BaseFee,
                    x.Currency,
                    x.FreeShippingFromAmount,
                    x.AllowCashOnDelivery,
                    x.RequiresCity,
                    x.RequiresPickupPoint,
                    x.RequiresStreetAddress,
                    x.EstimatedDaysMin,
                    x.EstimatedDaysMax,
                    x.Title,
                    x.Description))
                .ToList());

        return Result<ShippingProfileDto>.Success(dto);
    }
}