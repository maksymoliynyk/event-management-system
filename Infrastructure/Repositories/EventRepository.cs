using System;
using System.Linq;

using Domain.Aggregates.Events;

namespace Infrastructure.Repositories;

internal class EventRepository : IEventRepository
{
    private readonly EventManagementContext _context;

    internal EventRepository(EventManagementContext context)
    {
        _context = context;
    }

    public void Create(Event @event)
    {
        _context.Events.Add(@event);
    }

    public void Delete(Event @event)
    {
        _context.Events.Remove(@event);
    }

    public Event GetById(Guid eventId, Guid userId)
    {
        return _context.Events.FirstOrDefault(e => e.OwnerId == userId && e.Id == eventId);
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }
}
