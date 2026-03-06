using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.ChangeMySellerSlug;

public sealed record ChangeMySellerSlugCommand(string Slug) : IRequest<Result>;
