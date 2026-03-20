using BazaR.Backend.Application.Payments.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Payments.Queries.GetPaymentById;

public sealed record GetPaymentByIdQuery(
    Guid PaymentId) : IRequest<Result<PaymentDto>>;