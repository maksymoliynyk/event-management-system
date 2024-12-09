using System;
using System.Linq;

using Domain.Aggregates.Events;
using Domain.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly EventManagementContext _context;

    public EventRepository(EventManagementContext context)
    {
        _context = context;
    }

    public void Create(Event @event)
    {
        _context.Events.Add(@event);
    }

    public Event GetById(Guid eventId)
    {
        var @event = _context.Events
            .Include(x => x.Attendees)
            .Include(x => x.RSVPs)
            .FirstOrDefault(e => e.Id == eventId);
        if (@event == null)
        {
            throw new ObjectNotFoundException(EntitiesErrorType.Event);
        }

        return @event;
    }

    public void Delete(Event @event)
    {
        _context.Events.Remove(@event);
    }

    public Event GetById(Guid eventId, Guid ownerId)
    {
        var @event = _context.Events
            .Include(x => x.Attendees)
            .Include(x => x.RSVPs)
            .FirstOrDefault(e => e.OwnerId == ownerId && e.Id == eventId);
        if (@event == null)
        {
            throw new ObjectNotFoundException(EntitiesErrorType.Event);
        }

        return @event;
    }

    public void Update(Event @event)
    {
        _context.Events.Update(@event);
    }
}