using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.UpdateMySellerContacts;

public sealed record UpdateMySellerContactsCommand(string? Email, string? Phone) : IRequest<Result>;
