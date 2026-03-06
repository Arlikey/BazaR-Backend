using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.ChangeMySellerSlug;

public sealed class ChangeMySellerSlugCommandHandler : IRequestHandler<ChangeMySellerSlugCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public ChangeMySellerSlugCommandHandler(ISellerRepository sellers, IUnitOfWork uow, ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(ChangeMySellerSlugCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        var slugRes = SellerSlug.Create(request.Slug);
        if (slugRes.IsFailure)
            return Result.Failure(slugRes.Error);

        var taken = await _sellers.SlugExistsAsync(slugRes.Value!, excludeSellerId: seller.Id, ct);
        if (taken)
            return Result.Failure(new Error("Seller.SlugTaken", "Slug is already taken."));

        var res = seller.ChangeSlug(request.Slug);
        if (res.IsFailure) return res;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
