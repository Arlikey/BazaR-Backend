using BazaR.Backend.Application.Abstractions.Persistence;
using BazaR.Backend.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BazaR.Backend.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    private readonly IMediator _mediator;

    public UnitOfWork(AppDbContext db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entitiesWithEvents = _db.ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .ToList();

        var events = entitiesWithEvents
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        var result = await _db.SaveChangesAsync(ct);

        foreach (var ev in events)
            await _mediator.Publish(ev, ct);

        foreach (var entry in entitiesWithEvents)
            entry.Entity.ClearDomainEvents();

        return result;
    }
}
