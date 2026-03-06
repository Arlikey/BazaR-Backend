using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.ReactivateSeller;

public sealed record ReactivateSellerCommand(Guid SellerId) : IRequest<Result>;

public sealed class ReactivateSellerCommandHandler : IRequestHandler<ReactivateSellerCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public ReactivateSellerCommandHandler(
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(ReactivateSellerCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        if (!_current.IsAdmin)
            return Result.Failure(new Error("Auth.Forbidden", "Admin only."));

        if (request.SellerId == Guid.Empty)
            return Result.Failure(new Error("Seller.IdRequired", "SellerId is required."));

        var seller = await _sellers.GetByIdAsync(new SellerId(request.SellerId), ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        var result = seller.Reactivate(adminUserId: _current.UserId);
        if (result.IsFailure) return result;


        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
