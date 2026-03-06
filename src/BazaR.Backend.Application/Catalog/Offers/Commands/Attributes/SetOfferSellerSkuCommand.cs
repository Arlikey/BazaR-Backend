using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Offers.Commands.Attributes;

public sealed record SetOfferSellerSkuCommand(Guid OfferId, string? SellerSku) : IRequest<Result>;