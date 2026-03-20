using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.PaymentProfiles.Commands.SetBankAccount;

public sealed record SetBankAccountCommand(
    string RecipientName,
    string Iban,
    string BankName,
    string TaxNumber,
    string? Swift,
    string? PurposeTemplate) : IRequest<Result>;