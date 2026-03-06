using BazaR.Backend.Domain.Common;
using MediatR;

namespace BazaR.Backend.Application.Sellers.Commands.SubmitForApproval;

public sealed record SubmitSellerForApprovalCommand : IRequest<Result>;
