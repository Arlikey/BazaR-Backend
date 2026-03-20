using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Shipping;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.CreateShippingProfile;

public sealed class CreateShippingProfileCommandHandler
    : IRequestHandler<CreateShippingProfileCommand, Result<Guid>>
{
    private readonly IShippingProfileRepository _profiles;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public CreateShippingProfileCommandHandler(
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

    public async Task<Result<Guid>> Handle(CreateShippingProfileCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result<Guid>.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result<Guid>.Failure(new Error("Seller.NotFound", "Seller was not found."));

        var existing = await _profiles.GetBySellerIdAsync(seller.Id, ct);
        if (existing is not null)
            return Result<Guid>.Success(existing.Id.Value);

        var createResult = ShippingProfile.Create(seller.Id);
        if (createResult.IsFailure)
            return Result<Guid>.Failure(createResult.Error);

        var profile = createResult.Value!;

        _profiles.Add(profile);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(profile.Id.Value);
    }
}