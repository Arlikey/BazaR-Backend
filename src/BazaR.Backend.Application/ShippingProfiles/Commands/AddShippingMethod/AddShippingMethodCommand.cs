using BazaR.Backend.Domain.Common;
using BazaR.Backend.Domain.ShippingProfiles;
using MediatR;

namespace BazaR.Backend.Application.ShippingProfiles.Commands.AddShippingMethod;

public sealed record AddShippingMethodCommand(
    ShippingMethodType MethodType,
    decimal BaseFee,
    string Currency,
    decimal? FreeShippingFromAmount,
    bool AllowCashOnDelivery,
    int? EstimatedDaysMin,
    int? EstimatedDaysMax,
    string? Title,
    string? Description) : IRequest<Result>;