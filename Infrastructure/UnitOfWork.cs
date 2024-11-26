using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;

using Infrastructure.Repositories;

namespace Infrastructure;
public interface IUnitOfWork : IDisposable
{
    IEventRepository Event { get; }
    Task SaveAsync(CancellationToken ct);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly EventManagementContext _context;

    internal UnitOfWork(EventManagementContext context)
    {
        _context = context;
    }

    private IEventRepository _eventRepository;

    public IEventRepository Event
    {
        get
        {
            _eventRepository ??= new EventRepository(_context);
            return _eventRepository;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async Task SaveAsync(CancellationToken ct)
    {
        _ = await _context.SaveChangesAsync(ct);
    }
}