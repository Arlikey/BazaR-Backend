using BazaR.Backend.Application.Abstractions.ReadModels;
using BazaR.Backend.Application.Abstractions.Repositories.ReadModels;
using BazaR.Backend.Application.Shippings.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Shippings.Queries.GetShippingById;

public sealed class GetShippingByIdQueryHandler
    : IRequestHandler<GetShippingByIdQuery, Result<ShippingDetailsDto>>
{
    private readonly IShippingReadRepository _readRepository;

    public GetShippingByIdQueryHandler(IShippingReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public async Task<Result<ShippingDetailsDto>> Handle(
        GetShippingByIdQuery request,
        CancellationToken ct)
    {
        var shipping = await _readRepository.GetByIdAsync(request.ShippingId, ct);
        if (shipping is null)
        {
            return Result<ShippingDetailsDto>.Failure(new Error(
                "Shipping.NotFound",
                "Shipping was not found."));
        }

        return Result<ShippingDetailsDto>.Success(shipping);
    }
}