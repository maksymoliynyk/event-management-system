using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Models.Database;

using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagementContext _context;

        public EventRepository(EventManagementContext context)
        {
            _context = context;
        }
        public async Task ChangeEventStatus(string id, EventStatus eventStatus, CancellationToken cancellationToken = default)
        {
            EventDTO eventToChange = await GetEventById(id, cancellationToken);
            if (eventToChange.Status == (int)eventStatus)
            {
                return;
            }
            eventToChange.Status = (int)eventStatus;
            return;
        }

        public async Task<string> CreateEvent(EventDTO newEvent, CancellationToken cancellationToken = default)
        {
            _ = await _context.Events.AddAsync(newEvent, cancellationToken);
            return newEvent.Id.ToString();
        }

        public async Task<EventDTO> DeleteEventById(string id, CancellationToken cancellationToken = default)
        {
            EventDTO eventToDelete = await _context.Events.FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
            if (eventToDelete == null)
            {
                return null;
            }
            _ = _context.Events.Remove(eventToDelete);
            return eventToDelete;
        }

        public async Task<EventDTO> GetEventById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Events
                            .Include(t => t.Owner)
                            .FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
        }

        public async Task UpdateEvent(EventDTO updatedEvent, CancellationToken cancellationToken = default)
        {
            EventDTO eventToUpdate = await GetEventById(updatedEvent.Id.ToString(), cancellationToken);
            if (eventToUpdate == null)
            {
                return;
            }
            eventToUpdate.Id = updatedEvent.Id;
            eventToUpdate.Title = updatedEvent.Title;
            eventToUpdate.Description = updatedEvent.Description;
            eventToUpdate.Date = updatedEvent.Date;
            eventToUpdate.Duration = updatedEvent.Duration;
            eventToUpdate.OwnerId = updatedEvent.OwnerId;
            eventToUpdate.Location = updatedEvent.Location;
            eventToUpdate.Status = updatedEvent.Status;
            return;
        }
    }
}