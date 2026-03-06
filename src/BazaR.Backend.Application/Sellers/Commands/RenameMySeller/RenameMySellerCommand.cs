using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.RenameMySeller;

public sealed record RenameMySellerCommand(string Name) : IRequest<Result>;
