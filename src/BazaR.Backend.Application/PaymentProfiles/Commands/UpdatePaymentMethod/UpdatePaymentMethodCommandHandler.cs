using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.UpdatePaymentMethod;

public sealed class UpdatePaymentMethodCommandHandler
    : IRequestHandler<UpdatePaymentMethodCommand, Result>
{
    private readonly IPaymentProfileRepository _profiles;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public UpdatePaymentMethodCommandHandler(
        IPaymentProfileRepository profiles,
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _profiles = profiles;
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(UpdatePaymentMethodCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller was not found."));

        var profile = await _profiles.GetBySellerIdAsync(seller.Id, ct);
        if (profile is null)
            return Result.Failure(new Error("PaymentProfile.NotFound", "Payment profile was not found."));

        var result = profile.UpdateMethod(
            request.MethodType,
            request.RequiresOnlineAuthorization,
            request.RequiresBankAccount,
            request.RequiresLiqPay,
            request.MinAmount,
            request.MaxAmount,
            request.Title,
            request.Description);

        if (result.IsFailure)
            return result;

        _profiles.Update(profile);
        await _uow.SaveChangesAsync(ct);

        return Result.Success();
    }
}