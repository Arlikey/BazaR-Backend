using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;

using BazaR.Backend.Application.Common.Abstractions;
using BazaR.Backend.Application.Offers;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Attributes;

public sealed class SetOfferDeliveryDaysCommandHandler : IRequestHandler<SetOfferDeliveryDaysCommand, Result>
{
    private readonly IOfferRepository _offers;
    private readonly ISellerRepository _sellers;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUser _current;

    public SetOfferDeliveryDaysCommandHandler(
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

    public async Task<Result> Handle(SetOfferDeliveryDaysCommand request, CancellationToken ct)
    {
        var access = await OfferAccess.GetMySellerAndOfferAsync(
            request.OfferId, _current, _sellers, _offers, ct);

        if (access.IsFailure) return Result.Failure(access.Error);

        var (_, offer) = access.Value;

        var res = offer.SetDeliveryDays(request.DeliveryDays);
        if (res.IsFailure) return res;

        _offers.Update(offer);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}