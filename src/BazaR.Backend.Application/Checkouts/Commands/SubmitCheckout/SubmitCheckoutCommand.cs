
using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SubmitCheckout;

public sealed record SubmitCheckoutCommand(Guid CheckoutId)
    : IRequest<Result<CheckoutSubmissionResult>>;