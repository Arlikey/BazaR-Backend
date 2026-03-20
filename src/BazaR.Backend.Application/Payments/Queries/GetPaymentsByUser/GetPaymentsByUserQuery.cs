using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentsByUser;

public sealed record GetPaymentsByUserQuery(
    Guid UserId) : IRequest<Result<IReadOnlyCollection<PaymentDto>>>;