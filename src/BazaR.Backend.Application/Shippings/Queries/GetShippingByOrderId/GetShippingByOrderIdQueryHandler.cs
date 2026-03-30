using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.GetShippingByOrderId;

public sealed class GetShippingByOrderIdQueryHandler
    : IRequestHandler<GetShippingByOrderIdQuery, Result<ShippingDetailsDto>>
{
    private readonly IShippingReadRepository _readRepository;

    public GetShippingByOrderIdQueryHandler(IShippingReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<Result<ShippingDetailsDto>> Handle(
        GetShippingByOrderIdQuery request,
        CancellationToken ct)
    {
        var shipping = await _readRepository.GetByOrderIdAsync(request.OrderId, ct);
        if (shipping is null)
        {
            return Result<ShippingDetailsDto>.Failure(new Error(
                "Shipping.NotFound",
                "Shipping for order was not found."));
        }

        return Result<ShippingDetailsDto>.Success(shipping);
    }
}