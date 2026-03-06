using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions; // <-- ICurrentUser тут (как у тебя раньше)
using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.ApproveSeller;

public sealed record ApproveSellerCommand(Guid SellerId) : IRequest<Result>;

public sealed class ApproveSellerCommandHandler : IRequestHandler<ApproveSellerCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public ApproveSellerCommandHandler(
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(ApproveSellerCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        if (!_current.IsAdmin)
            return Result.Failure(new Error("Auth.Forbidden", "Admin only."));

        if (request.SellerId == Guid.Empty)
            return Result.Failure(new Error("Seller.IdRequired", "SellerId is required."));

        var sellerId = new SellerId(request.SellerId);

        var seller = await _sellers.GetByIdAsync(sellerId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        var approveRes = seller.Approve(_current.UserId);
        if (approveRes.IsFailure)
            return approveRes;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
