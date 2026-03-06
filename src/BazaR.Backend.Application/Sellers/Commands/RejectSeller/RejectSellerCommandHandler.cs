namespace BazaR.Backend.Application.Sellers.Commands.RejectSeller;

using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;

using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.Sellers;
using MediatR;

public sealed record RejectSellerCommand(Guid SellerId, string Reason) : IRequest<Result>;

public sealed class RejectSellerCommandHandler : IRequestHandler<RejectSellerCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public RejectSellerCommandHandler(ISellerRepository sellers, IUnitOfWork uow, ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(RejectSellerCommand request, CancellationToken ct)
    {
        if (!_current.IsAdmin)
            return Result.Failure(new Error("Auth.Forbidden", "Admin only."));

        var seller = await _sellers.GetByIdAsync(new SellerId(request.SellerId), ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        var res = seller.Reject(_current.UserId, request.Reason);
        if (res.IsFailure) return res;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
