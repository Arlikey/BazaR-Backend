using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.SuspendSeller;

public sealed record SuspendSellerCommand(
    Guid SellerId,
    string? Reason
) : IRequest<Result>;

public sealed class SuspendSellerCommandHandler
    : IRequestHandler<SuspendSellerCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SuspendSellerCommandHandler(
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SuspendSellerCommand request, CancellationToken ct)
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

        var result = seller.Suspend(
            adminUserId: _current.UserId,
            reason: request.Reason
        );

        if (result.IsFailure) return result;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

}
