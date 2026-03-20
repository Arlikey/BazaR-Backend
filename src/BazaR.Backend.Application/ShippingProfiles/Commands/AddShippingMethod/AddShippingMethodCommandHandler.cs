using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Users;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.AddShippingMethod;

public sealed class AddShippingMethodCommandHandler
    : IRequestHandler<AddShippingMethodCommand, Result>
{
    private readonly IShippingProfileRepository _profiles;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public AddShippingMethodCommandHandler(
        IShippingProfileRepository profiles,
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _profiles = profiles;
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(AddShippingMethodCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller was not found."));

        var profile = await _profiles.GetBySellerIdAsync(seller.Id, ct);
        if (profile is null)
            return Result.Failure(new Error("ShippingProfile.NotFound", "Shipping profile was not found."));

        var result = profile.AddMethod(
            request.MethodType,
            request.BaseFee,
            request.Currency,
            request.FreeShippingFromAmount,
            request.AllowCashOnDelivery,
            request.EstimatedDaysMin,
            request.EstimatedDaysMax,
            request.Title,
            request.Description);

        if (result.IsFailure)
            return result;

        _profiles.Update(profile);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}