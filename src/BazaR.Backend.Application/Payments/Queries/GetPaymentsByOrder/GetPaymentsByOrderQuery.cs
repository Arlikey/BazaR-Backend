using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentsByOrder;

public sealed record GetPaymentsByOrderQuery(
    Guid OrderId) : IRequest<Result<IReadOnlyCollection<PaymentDto>>>;