using BazaR.Backend.Application.Sellers.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Queries.GetMySeller;

public sealed record GetMySellerQuery : IRequest<Result<SellerDto>>;
