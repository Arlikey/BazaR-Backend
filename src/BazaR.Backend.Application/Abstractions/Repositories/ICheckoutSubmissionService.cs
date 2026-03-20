using BazaR.Backend.Application.Checkouts.DTOs;
using BazaR.Backend.Domain.Checkouts;
using BazaR.Backend.Domain.Common;

namespace BazaR.Backend.Application.Abstractions.Checkouts;

public interface ICheckoutSubmissionService
{
    Task<Result<CheckoutSubmissionResult>> SubmitAsync(
        Checkout checkout,
        DateTimeOffset nowUtc,
        CancellationToken ct);
}