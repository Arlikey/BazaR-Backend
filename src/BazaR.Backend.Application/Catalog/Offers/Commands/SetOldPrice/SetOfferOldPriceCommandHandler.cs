using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Offers;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.SetOldPrice;

public sealed class SetOfferOldPriceCommandHandler : IRequestHandler<SetOfferOldPriceCommand, Result>
{
    private readonly IOfferRepository _offers;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetOfferOldPriceCommandHandler(
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

    public async Task<Result> Handle(SetOfferOldPriceCommand request, CancellationToken ct)
    {
        var access = await OfferAccess.GetMySellerAndOfferAsync(
            request.OfferId, _current, _sellers, _offers, ct);

        if (access.IsFailure) return Result.Failure(access.Error);

        var (_, offer) = access.Value;

        // если валюта не указана — используем валюту текущей price, иначе UAH
        var cur = !string.IsNullOrWhiteSpace(request.Currency)
            ? request.Currency!.Trim().ToUpperInvariant()
            : offer.Price?.Currency ?? "UAH";

        var res = offer.SetOldPrice(request.Amount, cur);
        if (res.IsFailure) return res;

        _offers.Update(offer);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}