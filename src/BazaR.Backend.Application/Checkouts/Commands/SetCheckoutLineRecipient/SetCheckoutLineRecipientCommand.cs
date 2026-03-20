using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Checkouts.Commands.SetCheckoutLineRecipient;

public sealed record SetCheckoutLineRecipientCommand(
    Guid CheckoutId,
    Guid LineId,
    string FirstName,
    string LastName,
    string Phone,
    string? Email,
    bool IsCustomerRecipient
) : IRequest<Result>;