using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.SubmitForApproval;

public sealed class SubmitSellerForApprovalHandler
    : IRequestHandler<SubmitSellerForApprovalCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SubmitSellerForApprovalHandler(
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SubmitSellerForApprovalCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        var res = seller.SubmitForApproval();
        if (res.IsFailure) return res;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
