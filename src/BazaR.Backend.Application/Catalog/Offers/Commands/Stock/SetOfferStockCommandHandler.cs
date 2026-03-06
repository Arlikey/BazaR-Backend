using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Offers;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Stock;

public sealed class SetOfferStockCommandHandler : IRequestHandler<SetOfferStockCommand, Result>
{
    private readonly IOfferRepository _offers;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetOfferStockCommandHandler(
        IOfferRepository offers,
        ISellerRepository sellers,
        IUnitOfWork uow,
        ICurrentUser current)
    {
        _offers = offers;
        _sellers = sellers;
        _uow = uow;
        _current = current;
    }

    public async Task<Result> Handle(SetOfferStockCommand request, CancellationToken ct)
    {
        var access = await OfferAccess.GetMySellerAndOfferAsync(
            request.OfferId, _current, _sellers, _offers, ct);

        if (access.IsFailure) return Result.Failure(access.Error);

        var (_, offer) = access.Value;

        var res = offer.SetStock(request.Stock);
        if (res.IsFailure) return res;

        _offers.Update(offer);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}