using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.UpdateMySellerContacts;

public sealed class UpdateMySellerContactsCommandHandler : IRequestHandler<UpdateMySellerContactsCommand, Result>
{
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public UpdateMySellerContactsCommandHandler(ISellerRepository sellers, IUnitOfWork uow, ICurrentUser current)
    {
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(UpdateMySellerContactsCommand request, CancellationToken ct)
    {
        if (!_current.IsAuthenticated)
            return Result.Failure(new Error("Auth.Required", "Authentication required."));

        var seller = await _sellers.GetByOwnerUserIdAsync(_current.UserId, ct);
        if (seller is null)
            return Result.Failure(new Error("Seller.NotFound", "Seller not found."));

        var res = seller.UpdateSupportContacts(request.Email, request.Phone);
        if (res.IsFailure) return res;

        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

