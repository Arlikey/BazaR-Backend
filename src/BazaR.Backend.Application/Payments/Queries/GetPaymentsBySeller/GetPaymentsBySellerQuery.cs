using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentsBySeller;

public sealed record GetPaymentsBySellerQuery(
    Guid SellerId) : IRequest<Result<IReadOnlyCollection<PaymentDto>>>;