using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Application.Abstractions.Repositories;
using BazaR.Backend.Domain.Sellers.Events;
using BazaR.Backend.Domain.Users;
using MediatR;

public sealed class SellerApprovedEventHandler : INotificationHandler<SellerApprovedEvent>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public SellerApprovedEventHandler(IUserRepository users, IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task Handle(SellerApprovedEvent e, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(new UserId(e.OwnerUserId), ct);
        if (user is null) return;

        var res = user.GrantRole(UserRole.Seller);
        if (res.IsFailure) return;

        await _uow.SaveChangesAsync(ct);
    }
}
